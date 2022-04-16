using System.Net;
using Brunozec.Common.Auth;
using Brunozec.Common.Cache;
using Brunozec.Common.ErrorLogging;
using Brunozec.Common.Extensions.Http.Extensions;
using Brunozec.Common.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic;

namespace Brunozec.Common.Extensions.Http.Middleware;

public class AuthMiddleware : IMiddleware
{
    private readonly IErrorLogging _errorLogging;
    private readonly ICacheRedis _cacheRedis;
    private readonly IAuthProvider _authProvider;
    private readonly string _accountKey;

    public AuthMiddleware(
        IErrorLogging errorLogging,
        ICacheRedis cacheRedis,
        IAuthProvider authProvider
    )
    {
        _errorLogging = errorLogging;
        _cacheRedis = cacheRedis;
        _authProvider = authProvider;
        _accountKey = ConfigHelper.GetSetting("CONTEXT:ACCOUNT:KEY");
    }

    public async Task InvokeAsync(Microsoft.AspNetCore.Http.HttpContext context, RequestDelegate next)
    {
        var token = context.Request.Headers[ConfigHelper.GetSetting("CONTEXT:ACCOUNT:AUTHORIZATION:HEADER")].FirstOrDefault()?.Split(" ").Last();

        await AttachUserToContext(context, token);

        await next(context);
    }

    private async Task AttachUserToContext(Microsoft.AspNetCore.Http.HttpContext context, string? jwtoken)
    {
        try
        {
            if (string.IsNullOrEmpty(jwtoken))
                throw new ArgumentNullException(nameof(jwtoken));

            var accountInfo = await AuthExensions.GetAccountInfoFromJWToken(jwtoken);

            if (accountInfo == null)
                throw new ArgumentNullException(nameof(accountInfo));

            var jwtIsBlackListed = await _cacheRedis.GetStringAsync<string>($"{CacheConstants.Authorization_JWTBlackList}{accountInfo.Login}:{accountInfo.Jwtoken}");

            if (jwtIsBlackListed != null)
            {
                context.Response.Clear();
                context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("Authentication failed. User and/or password are incorrect");
                return;
            }

            var validation = await _authProvider.Authorize(accountInfo.Login, jwtoken)
                .Then(authorizedAccountInfo =>
                {
                    authorizedAccountInfo.Jwtoken = jwtoken;
                    authorizedAccountInfo.SessionId = accountInfo.SessionId;
                    context.Items[_accountKey] = authorizedAccountInfo;

                    return Task.CompletedTask;
                });

            if (!validation.IsValid)
            {
                context.Response.Clear();
                context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("Authentication failed. User and/or password are incorrect");
            }
        }
        catch (Exception ex)
        {
            context.Response.Clear();
            context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync("Authentication failed. User and/or password are incorrect");
            _errorLogging.Log(ex);
        }
    }
}