using System.Text.Json.Serialization;
using Tessa.Application.Enums;
using static Tessa.Application.Models.AppSettings;

namespace Tessa.Application.Models.ProviderConfigs;

public class ProviderConfigJan : ProviderConfigOpenAI
{
	public ProviderConfigJan()
	{
		Name = "jan";
		Provider = LlmProvider.Jan;
		ApiHostUrl = "http://localhost:1337";
		ApiKey = "";
	}
}