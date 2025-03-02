using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace backend_challenge.Models;

/// <summary>
/// Data transfer object (DTO) for a book, used for validating and transferring book data.
/// </summary>
public class BookDTO
{
    [Required(ErrorMessage = "Title is required")]
    [JsonPropertyName("title")]
    public string title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Author/Authors is required")]
    [JsonPropertyName("author")]
    public string author { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required")]
    [JsonPropertyName("description")]
    public string? description { get; set; }


    [Required(ErrorMessage = "Published year is required")]
    [CurrentYearMaximum]
    [JsonPropertyName("published_year")]
    public int publishedYear { get; set; }


    // <summary>
    /// Converts the current DTO instance to a <see cref="Book"/> model.
    /// </summary>
    /// <returns>A new instance of the <see cref="Book"/> model populated with the values from the DTO.</returns>
    public Book ToBook()
    {
        return new Book
        {
            id = Guid.NewGuid().ToString(),
            title = this.title,
            author = this.author,
            description = this.description,
            publishedYear = this.publishedYear
        };
    }

    /// <summary>
    /// Custom validation attribute that ensures the year is between 1 and the current year.
    /// </summary>
    private class CurrentYearMaximumAttribute : ValidationAttribute
    {
        /// <summary>
        /// Validates if the provided year is valid (between 1 and the current year).
        /// </summary>
        /// <param name="value">The year to validate.</param>
        /// <param name="validationContext">Context information about the validation.</param>
        /// <returns>A validation result indicating whether the year is valid.</returns>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is int year)
            {
                int currentYear = DateTime.Now.Year;

                if (year <= 0 || year > currentYear)
                {
                    return new ValidationResult($"Year must be between 1 and the current year {currentYear}");
                }

            }

            return ValidationResult.Success;
        }
    }
}
