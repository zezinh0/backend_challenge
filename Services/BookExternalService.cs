using System;
using System.Text.Json;
using backend_challenge.Models;
using backend_challenge.Models.External;
using Microsoft.Extensions.Caching.Memory;

namespace backend_challenge.Services;

/// <summary>
/// Service responsible for interacting with the external Open Library API to search for books and retrieve detailed information about books.
/// </summary>
/// <param name="httpClient">The HTTP client used to make requests to the external API.</param>
/// <param name="cache">The memory cache used for storing book data.</param>
/// <param name="logger">The logger used for logging service activities.</param>
public class BookExternalService(HttpClient httpClient, IMemoryCache cache, ILogger<BookExternalService> logger) : IBookExternalService
{

    private readonly HttpClient httpClient = httpClient;
    private readonly IMemoryCache cache = cache;
    private readonly ILogger<BookExternalService> logger = logger;
    private const string api_url = "https://openlibrary.org/";

    /// <summary>
    /// Searches for books from the external API based on the given parameters (title, author, page, pageSize).
    /// </summary>
    /// <param name="title">The title of the book to search for.</param>
    /// <param name="author">The author of the book to search for.</param>
    /// <param name="page">The page number to fetch.</param>
    /// <param name="pageSize">The number of results per page.</param>
    /// <returns>A list of books matching the search criteria, can be a [] .</returns>
    public async Task<IEnumerable<Book>> SearchBooksAsync(string? title, string? author, int? page = 1, int? pageSize = 10)
    {
        logger.LogInformation("Searching books with title: {Title}, author: {Author}, page: {Page}, pageSize: {PageSize}",
            title ?? "null", author ?? "null", page, pageSize);

        var query = "";
        if (!string.IsNullOrWhiteSpace(title))
            query += $"title={Uri.EscapeDataString(title)}";

        if (!string.IsNullOrWhiteSpace(author))
        {
            if (!string.IsNullOrWhiteSpace(query))
                query += "&";
            query += $"author={Uri.EscapeDataString(author)}";
        }

        page = page > 0 ? page : 1;
        pageSize = pageSize > 9 ? pageSize : 10;

        query += $"&page={page}&limit={pageSize}";

        var cacheKey = $"search_{query}";
        if (cache.TryGetValue(cacheKey, out IEnumerable<Book>? cachedBooks))
        {
            logger.LogInformation("Returning cached books for query: {Query}", query);
            return cachedBooks ?? Enumerable.Empty<Book>();
        }

        try
        {
            var url = $"{api_url}/search.json?{query}";
            logger.LogDebug("Making API request to: {Url}", url);

            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var searchResult = JsonSerializer.Deserialize<ExternalAPIResponse>(content);

            if (searchResult?.Docs == null || !searchResult.Docs.Any())
            {
                logger.LogInformation("No books found for query: {Query}", query);
                return Enumerable.Empty<Book>();
            }

            var books = searchResult.Docs.Select(doc => new Book
            {
                id = doc.Key.Replace("/works/", ""),
                title = doc.Title,
                author = doc.AuthorNames != null && doc.AuthorNames.Any()
                    ? string.Join(", ", doc.AuthorNames) : "Unknown",
                description = doc.GetDescription(),
                publishedYear = doc.FirstPublishYear ?? 0
            }).ToList();

            SetCache(cacheKey, books);

            return books;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during book search for query: {Query}", query);
            throw;
        }
    }

    /// <summary>
    /// Retrieves detailed information about a book by its ID from the external API.
    /// </summary>
    /// <param name="id">The ID of the book to retrieve.</param>
    /// <returns>The detailed information of the book if found, otherwise null.</returns>
    public async Task<Book?> GetBookByIdAsync(string id)
    {
        
        var cacheKey = $"works/{id}";
        if (cache.TryGetValue(cacheKey, out Book? cachedBook))
        {
            logger.LogInformation("Returning cached book for ID: {Id}", id);
            return cachedBook;
        }

        try
        {
            var url = $"{api_url}works/{id}.json";
            logger.LogDebug("Making API request to: {Url}", url);

            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var bookDetailResult = JsonSerializer.Deserialize<OpenBookDetail>(content);

            if (bookDetailResult == null)
            {
                logger.LogInformation("No book found for ID: {Id}", id);
                return null;
            }

            var bookDetail = new Book
            {
                id = id,
                title = bookDetailResult.Title,
                author = bookDetailResult.Authors != null && bookDetailResult.Authors.Any()
                    ? await GetAuthorByIdAsync(bookDetailResult.GetAuthors())
                    : "Unknown",
                description = bookDetailResult.GetDescription(),
                publishedYear = bookDetailResult.ParsePublishedYear(),
            };

            SetCache(cacheKey, bookDetail);

            return bookDetail;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get book details for ID: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Retrieves author details by their ID from the external API.
    /// </summary>
    /// <param name="authors">The list of author IDs.</param>
    /// <returns>A string containing the author names.</returns>
    private async Task<string> GetAuthorByIdAsync(List<string> authors)
    {
        var authorNames = new List<string>();

        foreach (var item in authors)
        {
            try
            {
                var cacheKey = $"authors/{item}";
                if (cache.TryGetValue(cacheKey, out string? cachedAuthor))
                {
                    if (cachedAuthor != null)
                    {
                        logger.LogDebug("Using cached author name for ID: {AuthorId}", item);
                        authorNames.Add(cachedAuthor);
                    }
                    continue;
                }

                var url = $"{api_url}authors/{item}.json";
                logger.LogDebug("Making API request to: {Url}", url);

                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var authorDetailResponse = JsonSerializer.Deserialize<OpenAuthorDetail>(content);

                string? nameToCache = authorDetailResponse?.Name ?? authorDetailResponse?.PersonalName;
                if (nameToCache != null)
                {
                    logger.LogDebug("Found author name: {Name} for ID: {AuthorId}", nameToCache, item);
                    SetCache(cacheKey, nameToCache);
                    authorNames.Add(nameToCache);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to get author details for ID: {AuthorId}", item);
                continue;
            }

        }

        var result = authorNames.Any() ? string.Join(", ", authorNames) : "Unknown";
        logger.LogDebug("Final author string: {Authors}", result);
        return result;
    }

    /// <summary>
    /// Sets a value in the memory cache.
    /// </summary>
    /// <typeparam name="T">The type of the value to cache.</typeparam>
    /// <param name="cacheKey">The key under which the value is cached.</param>
    /// <param name="value">The value to cache.</param>
    private void SetCache<T>(string cacheKey, T value)
    {
        try
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(1));
            cache.Set(cacheKey, value, cacheEntryOptions);
            logger.LogDebug("Cached value for key: {CacheKey}", cacheKey);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to set cache for key: {CacheKey}", cacheKey);
        }
    }
}
