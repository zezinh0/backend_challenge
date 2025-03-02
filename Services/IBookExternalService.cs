using System;
using backend_challenge.Models;

namespace backend_challenge.Services;

public interface IBookExternalService
{
    Task<IEnumerable<Book>> SearchBooksAsync(string? title, string? author, int? page = 1, int? pageSize = 10);
    Task<Book?> GetBookByIdAsync(string id);
}
