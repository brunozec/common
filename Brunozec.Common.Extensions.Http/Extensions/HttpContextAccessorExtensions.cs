using Brunozec.Common.Auth;
using Brunozec.Common.Helpers;
using Brunozec.Common.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Brunozec.Common.Extensions.Http.Extensions;

public static class HttpContextAccessorExtensions
{
    public static IAccountInfo? GetUserAccountInfo(this IHttpContextAccessor httpContextAccessor)
    {
        var accountData = httpContextAccessor.HttpContext?.Items[ConfigHelper.GetSetting("CONTEXT:ACCOUNT:KEY")];

        return accountData as IAccountInfo;
    }

    public static string? GetAuthorizationHeaderValue(this IHttpContextAccessor httpContextAccessor)
    {
        var authorization = httpContextAccessor?.HttpContext?.Request.Headers[ConfigHelper.GetSetting("CONTEXT:ACCOUNT:AUTHORIZATION:HEADER")].FirstOrDefault();

        if (authorization == null)
        {
            throw new Exception("Auth not found");
        }

        return authorization.Split(" ").LastOrDefault();
    }

    public static string? GetUserAuthJwt(this IHttpContextAccessor httpContextAccessor)
    {
        var account = httpContextAccessor.GetUserAccountInfo();

        return account?.Jwtoken;
    }

    public static string GetUserIpAddress(this IHttpContextAccessor httpContextAccessor)
    {
        if (httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress != null)
            return httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

        throw new Exception("Failed to load user from context");
    }

    public static ObjectResult ReturnAPI<T>(this T? o) where T : class
    {
        if (o == null)
        {
            return new NotFoundObjectResult(new FunctionResult<T>());
        }

        return new OkObjectResult(new FunctionResult<T>(o));
    }

    public static ObjectResult ReturnAPI<T>(this FunctionResult<T> FunctionResult)
    {
        if (FunctionResult.IsValid)
        {
            if (FunctionResult.Result == null)
            {
                return new NotFoundObjectResult(FunctionResult);
            }

            return new OkObjectResult(FunctionResult);
        }

        return new BadRequestObjectResult(FunctionResult);
    }

    public static async Task<ObjectResult> ReturnAPI<T>(this Task<FunctionResult<T>> FunctionResultAsync)
    {
        var FunctionResult = await FunctionResultAsync;

        if (FunctionResult.IsValid)
        {
            if (FunctionResult.Result == null)
            {
                return new NotFoundObjectResult(FunctionResult);
            }

            return new OkObjectResult(FunctionResult);
        }

        return new BadRequestObjectResult(FunctionResult);
    }

}