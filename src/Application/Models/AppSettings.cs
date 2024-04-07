using System.Text.Json.Serialization;
using Tessa.Application.Enums;

namespace Tessa.Application.Models;

public class AppSettings
{
	public class Defaults
	{
		public const string SettingsPath = "tessa.settings.json";
	}
	
	[JsonIgnore]
	public List<string> Errors { get; set; } = new();
	
	[JsonIgnore]
	public string SettingsPath { get; set; } = Defaults.SettingsPath;

	public OcrSettings Ocr { get; set; } = new();
        public class OcrSettings
	{
		public class Defaults
		{
			public const string InputPath = "../input";
			public const string OutputPath = "../output";
			public const OcrEngine Engine = OcrEngine.Tesseract;
			public const string TessdataLanguage = "eng";
		}

		[JsonPropertyName("in")]
		public string InputPath { get; set; } = Defaults.InputPath;

		[JsonPropertyName("out")]
		public string OutputPath { get; set; } = Defaults.OutputPath;

		[JsonPropertyName("engine")]
		public OcrEngine Engine { get; set; } = Defaults.Engine;

		[JsonPropertyName("lang")]
		public string LanguageTessdata { get; set; } = Defaults.TessdataLanguage;
	}
}
