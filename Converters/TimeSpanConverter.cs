using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ApiExamen.Converters
{
    public class TimeSpanConverter : JsonConverter<TimeSpan>
    {
        private const string FormatString = @"hh\:mm\:ss";

        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var stringValue = reader.GetString();
            if (TimeSpan.TryParseExact(stringValue, FormatString, null, out var value))
            {
                return value;
            }
            throw new JsonException($"Unable to convert \"{stringValue}\" to TimeSpan");
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(FormatString));
        }
    }
}
