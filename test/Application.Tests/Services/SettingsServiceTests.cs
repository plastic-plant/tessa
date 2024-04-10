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
					TessdataLanguage = "eng"
				}
			};
			var json = JsonSerializer.Serialize(settings);
			var existingFileName = "tessa.settings.json";
			File.WriteAllText(existingFileName, json);
			
			service.Load(existingFileName);
			
			Assert.Equal(settings.Ocr.InputPath, service.Settings.Ocr.InputPath);
			Assert.Equal(settings.Ocr.OutputPath, service.Settings.Ocr.OutputPath);
			Assert.Equal(settings.Ocr.Engine, service.Settings.Ocr.Engine);
			Assert.Equal(settings.Ocr.TessdataLanguage, service.Settings.Ocr.TessdataLanguage);
		}

		[Fact]
		public void LoadAppSettingsFromFile_NotFound_Throws()
		{
			var expectedError = "Could not load settings from [fullpath] not.existing.settings.json. Please run tessa config to create a new tessa.config.json.";
			
			var notExistingFile = "not.existing.settings.json";
			var sut = new SettingsService();

			sut.Load(notExistingFile);

			Assert.EndsWith(expectedError.Substring(expectedError.Length - 58), sut.Settings.Errors.First());
		}

		[Fact]
		public void LoadAppSettingsFromFile_InvalidJson_Throws()
		{
			var expectedError = "Could not load settings from [fullpath] invalid.settings.json. There's an error in the formatting of the JSON content. Please run tessa config to fix.";
			var invalidJson = "{ this is invalid json }";
			var filename = "invalid.settings.json";
			File.WriteAllText(filename, invalidJson);
			var sut = new SettingsService();

			sut.Load(filename);

			Assert.StartsWith(expectedError.Substring(0, 29), sut.Settings.Errors.First());
			Assert.EndsWith(expectedError.Substring(expectedError.Length - 88), sut.Settings.Errors.First());
		}
	}
}
