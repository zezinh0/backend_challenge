using backend_challenge.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using backend_challenge.Services;
using System.Text.Json;


/// <summary>
/// The BooksController class provides endpoints for managing books.
/// </summary>
namespace backend_challenge.Controllers
{

    [ApiController]
    [Route("books")]
    public class BooksController(IBookLocalService bookService, IBookExternalService bookExternalService, ILogger<BooksController> logger) : ControllerBase
    {
        private readonly IBookLocalService bookService = bookService;
        private readonly IBookExternalService bookExternalService = bookExternalService;
        private readonly ILogger<BooksController> logger = logger;

        /// <summary>
        /// Retrieves all books stored locally.
        /// </summary>
        /// <returns> A list of books. </returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetAllBooks()
        {

            logger.LogInformation("Getting all books");

            var books = await bookService.GetAllBooksAsync();

            logger.LogInformation("Books retrieved successfully");
            return Ok(books);


        }

        /// <summary>
        /// Adds a new book to the local storage.
        /// </summary>
        /// <param name="bookDTO">The book data transfer object containing book details.</param>
        /// <returns>The updated list of books.</returns> 
        [HttpPost]
        public async Task<ActionResult<IEnumerable<Book>>> AddBook([FromBody] BookDTO bookDTO)
        {
            logger.LogInformation("Adding book");

            if (bookDTO == null)
            {
                logger.LogWarning("Null book DTO received");
                return BadRequest("Book data is required");
            }

            var books = await bookService.AddBookAsync(bookDTO);
            logger.LogInformation("Book added successfully");
            return Ok(books);

        }

        /// <summary>
        /// Searches for books using optional query parameters.
        /// </summary>
        /// <param name="title">Optional book title to search for.</param>
        /// <param name="author">Optional author name to search for.</param>
        /// <param name="page">Optional page number for pagination.</param>
        /// <param name="pageSize">Optional number of results per page.</param>
        /// <returns>A list of books matching the search criteria.</returns>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Book>>> SearchBooks([FromQuery] string? title, [FromQuery] string? author, [FromQuery] int? page, [FromQuery] int? pageSize)
        {
            logger.LogInformation(
            "Searching books with title: {Title}, author: {Author}, page: {Page}, pageSize: {PageSize}",
            title ?? "null", author ?? "null", page, pageSize);

            var books = await bookExternalService.SearchBooksAsync(title, author, page, pageSize);

            logger.LogInformation("Books retrieved successfully");

            return Ok(books);

        }

        /// <summary>
        /// Retrieves a book by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the book.</param>
        /// <returns>The book if found, otherwise a 404 Not Found response.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBookById(string id)
        {
            logger.LogInformation("Getting book by ID: {Id}", id);
            var book = await bookExternalService.GetBookByIdAsync(id);

            if (book == null)
            {
                logger.LogInformation("Book with ID {Id} not found", id);
                return NotFound($"Book with ID {id} Not Found.");
            }

            logger.LogInformation("Book retrieved successfully");
            return Ok(book);

        }
    }
}
