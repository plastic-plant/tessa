using System.Text.Json.Serialization;
using Tessa.Application.Enums;
using Tessa.Application.Models.ProviderConfigs;

namespace Tessa.Application.Models;

public class AppSettings
{
	public class Defaults
	{
		public const string SettingsPath = "tessa.settings.json";
		public const string LogPath = "logs";
	}
	
	[JsonIgnore]
	public List<string> Errors { get; set; } = new();
	
	[JsonIgnore]
	public string SettingsPath { get; set; } = Defaults.SettingsPath;

	[JsonPropertyName("log")]
	public string LogPath { get; set; } = Defaults.LogPath;

	public OcrSettings Ocr { get; set; } = new();

    public class OcrSettings
	{
		public class Defaults
		{
			public const string InputPath = "input";
			public const string OutputPath = "output";
			public const OcrEngine Engine = OcrEngine.Tesseract;
			public const string TessdataPath = "tessdata";
			public const string TessdataLanguage = "eng";
			public const string ModelsPath = "models";
			public const string SelectedProviderConfigName = "geitje-7b-ultra";
			public const int MaxPrompt = 500;
			public const string CleanupPrompt = "Clean up text. Avoid confirmation messages in your response. If you can't provide an answer, leave the response empty. Answer in the language of given text. Here's the text: <CONTENT>\n";
		}

		[JsonPropertyName("in")]
		public string InputPath { get; set; } = Defaults.InputPath;

		[JsonPropertyName("out")]
		public string OutputPath { get; set; } = Defaults.OutputPath;		

		[JsonPropertyName("engine")]
		public OcrEngine Engine { get; set; } = Defaults.Engine;

		[JsonPropertyName("tessdata")]
		public string TessdataPath { get; set; } = Defaults.TessdataPath;

		[JsonPropertyName("lang")]
		public string TessdataLanguage { get; set; } = Defaults.TessdataLanguage;

		[JsonPropertyName("models")]
		public string ModelsPath { get; set; } = Defaults.ModelsPath;

		[JsonPropertyName("llm")]
		public string SelectedProviderConfigName { get; set; } = Defaults.SelectedProviderConfigName;
    }

    public LlmSettings Llm { get; set; } = new();

	public class LlmSettings
	{
		public class Defaults
		{
			public const string ModelsPath = "models";
		}

		[JsonPropertyName("models")]
		public string ModelsPath { get; set; } = Defaults.ModelsPath;

		[JsonPropertyName("providers")]
		public List<ProviderConfig> ProviderConfigurations { get; set; } = new();
	}
	
	public IProviderConfig? GetSelectedProviderConfiguration() => Llm.ProviderConfigurations.FirstOrDefault(config => string.Equals(config.Name, this.Ocr.SelectedProviderConfigName, StringComparison.OrdinalIgnoreCase)) ?? new ProviderConfig { Provider = LlmProvider.None };
}
