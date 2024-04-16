using System.Text.Json;
using System.Text.Json.Serialization;
using Tessa.Application.Enums;

namespace Tessa.Application.Models.ProviderConfigs;

public class ProviderConfigConverter : JsonConverter<ProviderConfig>
{

	public override ProviderConfig? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		using var document = JsonDocument.ParseValue(ref reader);
		var root = document.RootElement;
		var json = root.GetRawText();

		var provider = LlmProvider.Unknown;
		var providerFound = root.GetProperty("provider").GetString();
		Enum.TryParse(providerFound, ignoreCase: true, out provider);
		return provider switch
		{
			LlmProvider.Jan => JsonSerializer.Deserialize<ProviderConfigJan>(json, options),
			LlmProvider.Llama => JsonSerializer.Deserialize<ProviderConfigLlamaGguf>(json, options),
			LlmProvider.LMStudio => JsonSerializer.Deserialize<ProviderConfigLMStudio>(json, options),
			LlmProvider.OpenAI => JsonSerializer.Deserialize<ProviderConfigOpenAI>(json, options),
			_ => JsonSerializer.Deserialize<ProviderConfig>(json, options)
		};
	}

	public override void Write(Utf8JsonWriter writer, ProviderConfig value, JsonSerializerOptions options)
	{
		if (value is null)
		{
			writer.WriteNullValue();
		}
		else
		{
			var type = value.GetType();
			JsonSerializer.Serialize(writer, value, type, options);
		}
	}
}