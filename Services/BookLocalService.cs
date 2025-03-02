using System;
using System.Text.Json;
using System.Collections.Concurrent;
using backend_challenge.Models;
using backend_challenge.Models.External;
using DotNetEnv;
using Microsoft.Extensions.Caching.Memory;

namespace backend_challenge.Services;

/// <summary>
/// Service responsible for managing books locally by loading, adding, and retrieving books from a local file (`books.json`).
/// </summary>
public class BookLocalService(ILogger<BookLocalService> logger) : IBookLocalService
{
    // Using ConcurrentDictionary for thread-safe operations without explicit locks
    private readonly object lockObj = new object();
    private readonly List<Book> booksList = new List<Book>();
    private readonly ILogger<BookLocalService> logger = logger;

    /// <summary>
    /// Initializes the service by loading the books from a local `books.json` file.
    /// </summary>
    /// <remarks>
    /// If the file does not exist or cannot be loaded, an empty collection is initialized. 
    /// </remarks>
    /// <returns>A task representing the asynchronous operation.</returns>
    public void Initialize()
    {
        try
        {
            if (File.Exists("books.json"))
            {
                var json = File.ReadAllText("books.json");
                var books = JsonSerializer.Deserialize<List<Book>>(json) ?? new List<Book>();
                booksList.Clear(); // Ensure the list is empty before loading
                booksList.AddRange(books);
            }
            else
            {
                logger.LogWarning("books.json file not found. Initializing with empty collection.");
                booksList.Clear();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading books.json");
            booksList.Clear();
        }
    }

    /// <summary>
    /// Retrieves all books currently stored in the local collection.
    /// </summary>
    /// <returns>A task representing the asynchronous operation, with the list of all books as the result.</returns>
    public Task<IEnumerable<Book>> GetAllBooksAsync()
    {
        try
        {
            return Task.FromResult<IEnumerable<Book>>(booksList);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving all books");
            throw;
        }
    }

    /// <summary>
    /// Adds a new book to the local collection.
    /// </summary>
    /// <param name="bookDTO">The DTO containing the book details to be added.</param>
    /// <returns>A task representing the asynchronous operation, with the updated list of books as the result.</returns>
    /// <remarks>
    /// The new book will be assigned an ID based on the current count of books in the collection.
    /// </remarks>
    public async Task<IEnumerable<Book>> AddBookAsync(BookDTO bookDTO)
    {
        try
        {
            lock (lockObj)
            {
                var newId = (booksList.Count + 1).ToString();
                var newBook = bookDTO.ToBook();
                newBook.id = newId;
                booksList.Add(newBook);

            }
            return await GetAllBooksAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding book: {Title}", bookDTO.title);
            throw;
        }
    }
}
