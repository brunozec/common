using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using RestSharp;

namespace Brunozec.Common.Helpers;

public sealed class DateTimeJsonConverter : System.Text.Json.Serialization.JsonConverter<DateTime>
{
    private readonly string _dateFormatString;

    public DateTimeJsonConverter(string dateFormatString)
    {
        _dateFormatString = dateFormatString;
    }

    public override DateTime Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) => DateTime.ParseExact(reader.GetString()!, _dateFormatString, CultureInfo.InvariantCulture);

    public override void Write(
        Utf8JsonWriter writer,
        DateTime dateTimeValue,
        JsonSerializerOptions options) => writer.WriteStringValue(dateTimeValue.ToString(_dateFormatString, CultureInfo.InvariantCulture));
}

public static class JsonHelper
{
    public static JsonSerializerOptions GetJsonSerializerOptions(JsonIgnoreCondition nullValueHandling = JsonIgnoreCondition.WhenWritingNull, JsonNamingPolicy propertyNamingPolicy = null, string dateFormatString = null
      , bool writeIndented = false)
    {
        var jsonSerializerOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = nullValueHandling
          , PropertyNamingPolicy = propertyNamingPolicy ?? JsonNamingPolicy.CamelCase
          , ReferenceHandler = ReferenceHandler.IgnoreCycles
          , WriteIndented = writeIndented
          , IncludeFields = true
          , NumberHandling = JsonNumberHandling.AllowReadingFromString
          , PropertyNameCaseInsensitive = true
        };

        if (!string.IsNullOrEmpty(dateFormatString))
            jsonSerializerOptions.Converters.Add(new DateTimeJsonConverter(dateFormatString));

        return jsonSerializerOptions;
    }
    public static string ToJson<T>(this T value, JsonIgnoreCondition nullValueHandling = JsonIgnoreCondition.WhenWritingNull, JsonNamingPolicy propertyNamingPolicy = null, string dateFormatString = null, bool writeIndented = false)
    {
        try
        {
            var jsonSerializerOptions = GetJsonSerializerOptions(nullValueHandling, propertyNamingPolicy, dateFormatString, writeIndented);

            return System.Text.Json.JsonSerializer.Serialize(value, jsonSerializerOptions);
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public static T FromJson<T>(this string json, JsonIgnoreCondition nullValueHandling = JsonIgnoreCondition.WhenWritingNull, JsonNamingPolicy propertyNamingPolicy = null, string dateFormatString = null, bool writeIndented = false)
    {
        try
        {
            var jsonSerializerOptions = GetJsonSerializerOptions(nullValueHandling, propertyNamingPolicy, dateFormatString, writeIndented);

            return System.Text.Json.JsonSerializer.Deserialize<T>(json, jsonSerializerOptions);
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public static T FromJson<T>(this RestResponse response,
        JsonIgnoreCondition nullValueHandling = JsonIgnoreCondition.WhenWritingNull,
        JsonNamingPolicy propertyNamingPolicy = null,
        string dateFormatString = null,
        bool writeIndented = false)
    {
        try
        {
            var jsonSerializerOptions = GetJsonSerializerOptions(nullValueHandling, propertyNamingPolicy, dateFormatString, writeIndented);

            return System.Text.Json.JsonSerializer.Deserialize<T>(response.Content, jsonSerializerOptions);
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public static T GetJsonElemetValue<T>(this object element)
    {
        if (element == null) return default(T);
        if (element.GetType() == typeof(JsonElement))
        {
            var jsonElement = ((JsonElement) element);

            return jsonElement.GetRawText().FromJson<T>();
        }

        return (T) element;
    }

    public static object GetJsonElemetValue(this object element)
    {
        if (element == null) return null;
        if (element.GetType() == typeof(JsonElement))
        {
            var jsonElement = ((JsonElement) element);

            switch (jsonElement.ValueKind)
            {
                case JsonValueKind.Number:
                    return jsonElement.GetDecimal();
                    break;
                case JsonValueKind.True:
                case JsonValueKind.False:
                    return jsonElement.GetBoolean();
                    break;
                case JsonValueKind.Object:
                case JsonValueKind.Undefined:
                default:

                    if (jsonElement.TryGetDateTime(out var resDat))
                        return resDat;
                    else
                        return jsonElement.GetString();
                    break;
            }
        }

        return element;
    }

    public static Dictionary<string, object> LoadColunasPersonalizadasFromJson(string json)
    {
        var expandoObject = new Dictionary<string, object>();
        if (json != null && !string.IsNullOrEmpty(json))
        {
            var colunas = json.FromJson<object>();
            if (colunas != null)
            {
                if (colunas.GetType() == typeof(JsonElement)
                    && ((JsonElement) colunas).ValueKind == JsonValueKind.Array)
                {
                    var colunasPersonalizadas = ((JsonElement) colunas).GetRawText().FromJson<IEnumerable<Dictionary<string, object>>>();

                    var dict = colunasPersonalizadas.FirstOrDefault() ?? new Dictionary<string, object>();

                    foreach (var colunasValores in dict)
                    {
                        expandoObject.Add(colunasValores.Key, GetJsonElemetValue(colunasValores.Value));
                    }
                }
            }
        }

        return expandoObject;
    }
}