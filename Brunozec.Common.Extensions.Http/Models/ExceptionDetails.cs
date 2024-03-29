﻿using System.Text.Json;

namespace Brunozec.Common.Extensions.Http.Models;

public sealed class ExceptionDetails
{
    public readonly int StatusCode;
    public readonly string Message;

    public ExceptionDetails(int statusCode, string message)
    {
        StatusCode = statusCode;
        Message = message ?? "No error message found in exception.";
    }

    public override string ToString() => JsonSerializer.Serialize(this);
}