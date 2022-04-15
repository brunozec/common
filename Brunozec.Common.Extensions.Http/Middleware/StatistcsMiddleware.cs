using Microsoft.AspNetCore.Http;

namespace Brunozec.Common.Extensions.Http.Middleware;

public class StatistcsMiddleware : Microsoft.AspNetCore.Http.IMiddleware
{
    public StatistcsMiddleware()
    {
    }

    public async Task InvokeAsync(Microsoft.AspNetCore.Http.HttpContext context, RequestDelegate next)
    {
        await next(context);
    }
}