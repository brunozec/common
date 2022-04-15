using System.IO.Compression;
using Brunozec.Common.Helpers;
using ElmahCore;
using ElmahCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Brunozec.Common.Extensions.Http.Helpers;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCorsConfig(this IServiceCollection services, string name)
    {
        services.AddCors(c => c.AddPolicy(name,
            options => options.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()));

        return services;
    }

    public static IServiceCollection AddResponseCompressionConfig(
        this IServiceCollection services,
        IConfiguration config,
        bool enableForHttps,
        string[] gzipMimeTypes,
        CompressionLevel compressionLvl = CompressionLevel.Optimal)
    {
        services.Configure<BrotliCompressionProviderOptions>(options => options.Level = compressionLvl);
        services.Configure<GzipCompressionProviderOptions>(options => options.Level = compressionLvl);

        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = enableForHttps;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
            options.MimeTypes = gzipMimeTypes;
        });

        return services;
    }

    public static IServiceCollection AddElmah<TElmahErrorLog>(
        this IServiceCollection service,
        string connectionString,
        string applicationName,
        string path = "admin/elmah",
        Func<Microsoft.AspNetCore.Http.HttpContext, bool> onPermissionCheck = null) where TElmahErrorLog : ErrorLog
    {
        return service.AddElmah<TElmahErrorLog>(options =>
        {
            options.Path = path;
            options.OnPermissionCheck = onPermissionCheck;
            options.ConnectionString = connectionString;
            options.ApplicationName = applicationName;
        });
    }
}