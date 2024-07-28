using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tessa.Application.Converters
{
	public class LowercaseJsonStringEnumConverter : JsonConverterFactory
	{
		public override bool CanConvert(Type typeToConvert)
		{
			return typeToConvert.IsEnum;
		}

		public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
		{
			Type converterType = typeof(LowercaseEnumConverter<>).MakeGenericType(typeToConvert);
			return (JsonConverter)Activator.CreateInstance(converterType);
		}

		private class LowercaseEnumConverter<T> : JsonConverter<T>
			where T : struct, Enum
		{
			public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				string value = reader.GetString();
				if (Enum.TryParse(typeof(T), value, ignoreCase: true, out var result))
				{
					return (T)result;
				}
				throw new JsonException($"Unable to convert \"{value}\" to Enum \"{typeof(T)}\".");
			}

			public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
			{
				string lowercaseValue = value.ToString().ToLower();
				writer.WriteStringValue(lowercaseValue);
			}
		}
	}
}
