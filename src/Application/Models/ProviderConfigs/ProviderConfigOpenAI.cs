﻿using System.Text.Json.Serialization;
using Tessa.Application.Enums;
using static Tessa.Application.Models.AppSettings;

namespace Tessa.Application.Models.ProviderConfigs;

public class ProviderConfigOpenAI : ProviderConfig
{
	[JsonPropertyName("host")]
	public string ApiHostUrl { get; set; }
	[JsonPropertyName("apikey")]
	public string? ApiKey { get; set; }
	public string Model { get; set; }
	public int MaxPrompt { get; set; }
	public string CleanupPrompt { get; set; }

	public ProviderConfigOpenAI()
    {
        Name = "openai";
		Provider = LlmProvider.OpenAI;
		ApiHostUrl = "https://api.openai.com";
		ApiKey = "YOUR_API_KEY";
		Model = "gpt-3.5-turbo";
		MaxPrompt = OcrSettings.Defaults.MaxPrompt;
		CleanupPrompt = OcrSettings.Defaults.CleanupPrompt;
	}
}
