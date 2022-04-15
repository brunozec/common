using Microsoft.AspNetCore.Http;

namespace Brunozec.Common.Extensions.Http;

public class BFZHttpContextAcessor
{
    private IAccountInfo _accountInfo;
    
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BFZHttpContextAcessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public IAccountInfo? GetUserAccountInfo()
    {
        return _httpContextAccessor.HttpContext == null ? _accountInfo : _httpContextAccessor?.GetUserAccountInfo();
    }
    
    public void SetAccountInfo(IAccountInfo accountInfo)
    {
        if (_httpContextAccessor.HttpContext != null)
            throw new Exception("Not possible to set account info in this context");

        _accountInfo = accountInfo;
    }
    
    public string GetUserIpAddress()
    {
        if (_httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress != null)
            return _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

        throw new Exception("Falha ao carregar informações do usuário logado");
    }

    public string[] GetContextHeader(string key)
    {
        return _httpContextAccessor?.HttpContext?.Request.Headers[key];
    }

    public string? GetAuthorizationHeaderValue()
    {
        return _httpContextAccessor?.GetAuthorizationHeaderValue();
    }
}