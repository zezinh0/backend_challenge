using System;
using System.Text.Json.Serialization;

namespace backend_challenge.Models.External;

/// <summary>
/// Represents the response returned by an external API when querying books.
/// </summary>
public class ExternalAPIResponse
{
    /// <summary>
    /// A list of books that match the search query.
    /// </summary>
    [JsonPropertyName("docs")]
    public List<BookSearch> Docs { get; set; } = new List<BookSearch>();

    /// <summary>
    /// The number of books found by the search query.
    /// </summary>
    [JsonPropertyName("numFound")]
    public int NumFound { get; set; }
}

/// <summary>
/// Represents a book search result returned by the external API.
/// </summary>
public class BookSearch
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("author_name")]
    public List<string> AuthorNames { get; set; } = new List<string>();

    [JsonPropertyName("first_publish_year")]
    public int? FirstPublishYear { get; set; }

    [JsonPropertyName("description")]
    public object? Description { get; set; }

    /// <summary>
    /// Gets the description of the book. Handles both string and object descriptions.
    /// </summary>
    /// <returns>The book description as a string.</returns>
    public string GetDescription()
    {
        if (Description == null)
            return string.Empty;

        if (Description is string strDescription)
            return strDescription;

        // Handle case where description is an object with a "value" property
        try
        {
            var jsonElement = (System.Text.Json.JsonElement)Description;
            if (jsonElement.TryGetProperty("value", out var value))
                return value.GetString() ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }

        return string.Empty;
    }
}

/// <summary>
/// Represents the detailed information about a specific book.
/// </summary>
public class OpenBookDetail
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("authors")]
    public List<object> Authors { get; set; } = new List<object>();

    [JsonPropertyName("description")]
    public object? Description { get; set; }

    [JsonPropertyName("first_publish_date")]
    public string? FirstPublishDate { get; set; }

    /// <summary>
    /// Gets the description of the book. Handles both string and object descriptions.
    /// </summary>
    /// <returns>The book description as a string.</returns>
    public string GetDescription()
    {
        if (Description == null)
            return string.Empty;

        if (Description is string strDescription)
            return strDescription;

        // Handle case where description is an object with a "value" property
        try
        {
            var jsonElement = (System.Text.Json.JsonElement)Description;
            if (jsonElement.TryGetProperty("value", out var value))
                return value.GetString() ?? string.Empty;

            return string.Empty;
        }
        catch
        {
            return string.Empty;
        }

    }

    /// <summary>
    /// Gets a list of author names from the authors field, which may contain nested objects.
    /// </summary>
    /// <returns>A list of author names, or empty list.</returns>
    public List<string> GetAuthors()
    {
        if (Authors == null || Authors.Count == 0)
            return [];

        // Handle case where description is an object with a "value" property
        try
        {
            List<string> authorslist = new List<string>();
            foreach (var item in Authors)
            {
                var item2 = (System.Text.Json.JsonElement)item;
                if (item2.TryGetProperty("author", out var author))
                    if (author.TryGetProperty("key", out var key))
                    {
                        var aux = key.GetString();
                        if (aux != null)
                            authorslist.Add(aux.Replace("/authors/", ""));

                    }
            }

            return authorslist;
        }
        catch
        {
            return [];
        }
    }

    /// <summary>
    /// Parses the first published date to extract the year.
    /// </summary>
    /// <returns>The 4-digit year of the first published date, or -1 if not available.</returns>
    public int ParsePublishedYear()
    {
        if (string.IsNullOrEmpty(FirstPublishDate))
            return -1;

        // Try to extract a 4-digit year from the publish date string
        var yearMatch = System.Text.RegularExpressions.Regex.Match(FirstPublishDate, @"\b(\d{4})\b");
        if (yearMatch.Success && int.TryParse(yearMatch.Groups[1].Value, out int year))
            return year;

        return -1;
    }


}

/// <summary>
/// Represents detailed information about an author.
/// </summary>
public class OpenAuthorDetail
{
    [JsonPropertyName("personal_name")]
    public string? PersonalName { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}