using System.Globalization;
using Brunozec.Common.ErrorLogging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Brunozec.Common.Extensions.Http.Middleware;

public class CultureMiddleware : Microsoft.AspNetCore.Http.IMiddleware
{
    private readonly IConfiguration _configuration;
    private readonly IErrorLogging _errorLogging;
    private readonly string _cultureCookie;
    private readonly string _cultureDefault;

    public CultureMiddleware(IConfiguration configuration, IErrorLogging errorLogging)
    {
        _configuration = configuration;
        _errorLogging = errorLogging;

        _cultureCookie = _configuration["CULTURE:COOKIE_NAME"];
        _cultureDefault = _configuration["CULTURE:DEFAULT"];
    }

    public async Task InvokeAsync(Microsoft.AspNetCore.Http.HttpContext context, RequestDelegate next)
    {
        try
        {
            var supportedCultures = new List<CultureInfo>
            {
                new("pt-BR")
                , new("en-US")
                , new("en")
                , new("es")
                ,
            };

            var cultureHeader = context.Request.Headers["Content-Language"].FirstOrDefault();

            string culture = "pt-BR";
            if (!string.IsNullOrEmpty(cultureHeader))
            {
                culture = cultureHeader;
            }
            else
            {
                var cultureCookie = context.Request.Cookies[_cultureCookie];

                if (string.IsNullOrEmpty(cultureCookie))
                    culture = _cultureDefault;
            }

            if (supportedCultures.All(c => c.Name != culture))
                culture = supportedCultures[0].Name;

            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(culture);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(culture);

            context.Response.Headers["Content-Language"] = culture;
        }
        catch (Exception ex)
        {
            _errorLogging.Log(ex);
        }

        await next(context);
    }
}