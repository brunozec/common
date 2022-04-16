using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Brunozec.Common.Auth;
using Brunozec.Common.Helpers;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Brunozec.Common.Extensions.Http.Extensions;

public static class AuthExensions
{
    public static Task<string?> GenerateJWToken(IAccountInfo accountInfo)
    {
        return GenerateJWToken(ConfigHelper.GetSetting("Secret"), accountInfo);
    }

    public static Task<string?> GenerateJWToken(string secret, IAccountInfo accountInfo)
    {
        try
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new[]
                    {
                        new Claim(ConfigHelper.GetSetting("CONTEXT:ACCOUNT:KEY"), JsonConvert.SerializeObject(accountInfo)), new Claim(ConfigHelper.GetSetting("CONTEXT:ACCOUNT:SESSION:ID"), accountInfo.SessionId.ToString())
                    })
                , Expires = DateTime.UtcNow.AddDays(Convert.ToDouble(ConfigHelper.GetSetting("AUTH:JWT:Duration")))
                , SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Task.FromResult(tokenHandler.WriteToken(token))!;
        }
        catch (Exception)
        {
            return Task.FromResult<string>(null);
        }
    }

    public static async Task<IAccountInfo?> GetAccountInfoFromJWToken(string? jwt)
    {
        return await GetAccountInfoFromJWToken(ConfigHelper.GetSetting("Secret"), ConfigHelper.GetSetting("CONTEXT:ACCOUNT:KEY"), jwt);
    }
    
    public static async Task<IAccountInfo?> GetAccountInfoFromJWToken(string secret, string accountKey, string? jwt)
    {
        return await GetAccountInfoFromJWToken(secret, accountKey, ConfigHelper.GetSetting("CONTEXT:ACCOUNT:SESSION:ID"), jwt);
    }

    private static Task<IAccountInfo?> GetAccountInfoFromJWToken(string secret, string accountKey, string sessionIdKey, string? jwt)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secret);
        tokenHandler.ValidateToken(jwt, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true
            , IssuerSigningKey = new SymmetricSecurityKey(key)
            , ValidateIssuer = false
            , ValidateAudience = false
            , ClockSkew = TimeSpan.Zero
            ,
        }, out SecurityToken validatedToken);

        var jwtToken = (JwtSecurityToken) validatedToken;
        var caccount = jwtToken.Claims.First(x => x.Type == accountKey).Value;
        var sessionId = jwtToken.Claims.First(x => x.Type == sessionIdKey).Value;

        var accountInfo = JsonConvert.DeserializeObject<IAccountInfo>(caccount);

        if (accountInfo == null)
        {
            return Task.FromResult<IAccountInfo>(null);
        }

        accountInfo.Jwtoken = jwt;
        accountInfo.SessionId = new Guid(sessionId);

        return Task.FromResult(accountInfo)!;
    }

    public static Task<bool> ValidateJWToken(string secret, string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true
                , IssuerSigningKey = new SymmetricSecurityKey(key)
                , ValidateIssuer = false
                , ValidateAudience = false
                ,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken) validatedToken;
            var caccount = jwtToken.Claims.First(x => x.Type == ConfigHelper.GetSetting("CONTEXT:ACCOUNT:KEY")).Value;

            return Task.FromResult(!string.IsNullOrEmpty(caccount));
        }
        catch (Exception)
        {
            return Task.FromResult(false);
        }
    }
    public static Task<string?> GetAuthorizationJWToken(string? authorization)
    {
        if (authorization == null) return Task.FromResult<string>(null);
        
        var values = authorization.Split(' ');

        if (values[0].ToLower() == "bearer")
        {
            return Task.FromResult(values[1])!;
        }

        return Task.FromResult<string>(null);
    }
}