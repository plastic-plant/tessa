using System.Text.Json;
using Tessa.Application.Enums;
using Tessa.Application.Models;
using Tessa.Application.Services;

namespace Application.Tests.Services
{
	public class SettingsServiceTests
	{
		[Fact]
		public void LoadAppSettingsFromFile_ExistingFile_Returns()
		{
			var service = new SettingsService();
			var settings = new AppSettings
			{
				Ocr = new AppSettings.OcrSettings
				{
					InputPath = "input",
					OutputPath = "output",
					Engine = OcrEngine.Tesseract,
					LanguageTessdata = "eng"
				}
			};
			var json = JsonSerializer.Serialize(settings);
			var existingFileName = "tessa.settings.json";
			File.WriteAllText(existingFileName, json);
			
			service.Load(existingFileName);
			
			Assert.Equal(settings.Ocr.InputPath, service.Settings.Ocr.InputPath);
			Assert.Equal(settings.Ocr.OutputPath, service.Settings.Ocr.OutputPath);
			Assert.Equal(settings.Ocr.Engine, service.Settings.Ocr.Engine);
			Assert.Equal(settings.Ocr.LanguageTessdata, service.Settings.Ocr.LanguageTessdata);
		}

		[Fact]
		public void LoadAppSettingsFromFile_NotFound_Throws()
		{
			var settingsService = new SettingsService();
			var notExistingFile = "not.existing.settings.json";

			Action action = () => settingsService.Load(notExistingFile);

			Assert.Throws<ArgumentException>(action);
		}

		[Fact]
		public void LoadAppSettingsFromFile_InvalidJson_Throws()
		{
			var settingsService = new SettingsService();
			var invalidJson = "{ \"ocr\": { \"input\": \"input\", \"output\": \"output\", \"engine\": \"tesseract\", \"lang\": \"eng\" } }";
			var invalidFileName = "invalid.settings.json";
			File.WriteAllText(invalidFileName, invalidJson);

			Action action = () => settingsService.Load(invalidFileName);

			Assert.Throws<JsonException>(action);
		}
	}
}
