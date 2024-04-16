using Tessa.Application.Enums;

namespace Tessa.Application.Models.ProviderConfigs;

public class ProviderConfigLMStudio : ProviderConfigOpenAI
{
    public ProviderConfigLMStudio()
    {
        Name = "lmstudio";
        Provider = LlmProvider.LMStudio;
        ApiHostUrl = "http://localhost:1234";
        ApiKey = "";
        Model = "";
    }
}