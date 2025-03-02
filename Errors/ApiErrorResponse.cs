using System;
using System.Collections.Generic;

namespace backend_challenge.Errors;

public class ApiErrorResponse(int statusCode, string message, string? details)
{
    public int StatusCode { get; set; } = statusCode;

    public string Message { get; set; } = message;

    public string? Details { get; set; } = details;
} 

