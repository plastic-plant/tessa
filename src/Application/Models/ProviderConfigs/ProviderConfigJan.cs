using Tessa.Application.Enums;

namespace Tessa.Application.Models.ProviderConfigs;

public class ProviderConfigJan : ProviderConfigOpenAI
{
    public ProviderConfigJan()
    {
        Name = "Jan";
		Provider = LlmProvider.LMStudio;
        ApiHostUrl = "http://localhost:1337";
        ApiKey = "";
	}
}