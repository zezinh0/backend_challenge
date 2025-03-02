using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace backend_challenge.Models;

public class Book
{

    [JsonPropertyName("id")]
    public required string id { get; set; } = string.Empty;


    [JsonPropertyName("title")]
    public required string title { get; set; } = string.Empty;


    [JsonPropertyName("author")]
    public required string author{ get; set; } = string.Empty;


    [JsonPropertyName("description")]
    public string? description { get; set; }


    [JsonPropertyName("published_year")]
    public required int? publishedYear { get; set; }
}
