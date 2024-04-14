using System.Text.Json.Serialization;
using Tessa.Application.Enums;

namespace Tessa.Application.Models.ProviderConfigs;

public class ProviderConfigLMStudio : ProviderConfigOpenAI
{
    [JsonConstructor]
    public ProviderConfigLMStudio()
    {
        Name = "lmstudio";
		Provider = LlmProvider.LMStudio;
        ApiHostUrl = "http://localhost:1234";
        ApiKey = "";
        Model = "";
	}
}