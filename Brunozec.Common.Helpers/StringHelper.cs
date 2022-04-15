using System.Text;

namespace Brunozec.Common.Helpers;

public static class StringHelper
{
    public static string EncodeBase64(this string s)
    {
        var bytes = Encoding.UTF8.GetBytes(s);
        return Convert.ToBase64String(bytes);
    }

    public static string DecodeBase64(this string s)
    {
        var bytes = Convert.FromBase64String(s);
        return Encoding.UTF8.GetString(bytes);
    }
}