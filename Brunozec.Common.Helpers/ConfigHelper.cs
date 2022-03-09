using Microsoft.Extensions.Configuration;

namespace Brunozec.Common.Helpers;

public class ConfigHelper
{
    private static IConfiguration _configuration;

    public static void Configure(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public static string GetSetting(string key)
    {
        var value = _configuration[key];

        if (!string.IsNullOrEmpty(value))
            return value;

        throw new Exception("App setting not found in appsettings: " + key);
    }

    public static T GetSetting<T>(string key)
    {
        return (T)Convert.ChangeType(GetSetting(key), typeof(T));
    }

    public static string GetConnectionString(string name)
    {
        return GetSetting<string>(name);
    }

    public static IConfigurationSection GetSection(string key)
    {
        return _configuration.GetSection(key);
    }
}