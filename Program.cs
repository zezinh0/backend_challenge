using backend_challenge.Services;
using backend_challenge.Middleware;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog as the global logger for the application.
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .WriteTo.File(
        path: "Logs/app-.txt",                          // Base filename
        rollingInterval: RollingInterval.Day,          // Create new file every day
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{Level:u3}] {Message:lj}{NewLine}{Exception}",
        retainedFileCountLimit: 7                       // Keep only last 7 days
    )
    .CreateLogger();

// This line swaps the default console logger for Serilog, redirecting all logs to the file configured above.
builder.Host.UseSerilog();

// Register services in the DI container for HTTP requests, caching, custom logic, and MVC controllers.
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IBookLocalService, BookLocalService>();
builder.Services.AddSingleton<IBookExternalService, BookExternalService>();
builder.Services.AddControllers();
// End of register services.

var app = builder.Build();

// Configure middleware in the HTTP request pipeline.
app.UseRequestLogging();          // Log incoming requests for monitoring and debugging.
app.UseExceptionMiddleware();     // Catch and handle exceptions, providing custom error responses.
if (!app.Environment.IsDevelopment()) 
{
    app.UseHttpsRedirection();    // Redirect HTTP requests to HTTPS for security.
}
// End of middleware configuration.

// Configure routing for the application.
app.MapControllers();

// Initialize the BookLocalService at application startup.
var bookService = app.Services.GetRequiredService<IBookLocalService>();
try
{
    bookService.Initialize();
    Log.Information("BookService initialized successfully");
}
catch (Exception ex)
{
    Log.Error(ex, "Error initializing BookService");
}

// Start the application.
app.Run();

