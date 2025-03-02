using System;
using backend_challenge.Models;

namespace backend_challenge.Services;

public interface IBookLocalService
{
    Task<IEnumerable<Book>> GetAllBooksAsync();
    Task<IEnumerable<Book>> AddBookAsync(BookDTO book);
    void Initialize();
}
