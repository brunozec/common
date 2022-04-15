using Brunozec.Common.Auth;

namespace Brunozec.Common.Extensions.Http.Abstraction;

public interface IBFZHttpContextAcessor
{
    IAccountInfo? GetUserAccountInfo();

    void SetAccountInfo(IAccountInfo? accountInfo);

    string GetUserIpAddress();

    string[] GetContextHeader(string key);

    string? GetAuthorizationHeaderValue();
}