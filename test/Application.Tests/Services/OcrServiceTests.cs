using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Tessa.Application.Enums;
using Tessa.Application.Interface;
using Tessa.Application.Interfaces;
using Tessa.Application.Models;
using Tessa.Application.Services;

namespace Application.Tests.Services
{
	public class OcrServiceTests
	{
		[Fact]
		public void Validate_WhenInputPathIsNotExisting_Error()
		{
			var logger = new Mock<ILogger<OcrService>>();
			var services = new Mock<IServiceProvider>();
			var settings = new Mock<ISettingsService>();
			var files = new Mock<IFileRepository>();
			settings
				.Setup(settings => settings.Settings)
				.Returns(new AppSettings { Ocr = new AppSettings.OcrSettings { InputPath = "path" } });
			files
				.Setup(files => files.GetPathSummary("path"))
				.Returns(new PathSummary { PathRooted = "//example", IsExistingDirectory = false, IsExistingFile = false });
			var sut = new OcrService(logger.Object, services.Object, settings.Object, files.Object);

			var actual = sut.Validate();

			Assert.NotEmpty(actual.Errors);
			Assert.Equal("Could not open folder or file //example.", actual.Errors.First());
		}

		[Fact]
		public void Validate_WhenEngineIsTesseractAndRepositoryIsNotReady_Error()
		{
			var logger = new Mock<ILogger<OcrService>>();
			var settings = new Mock<ISettingsService>();
			var files = new Mock<IFileRepository>();
			var tesseract = new Mock<ITesseractOcrRepository>();
			settings
				.Setup(settings => settings.Settings)
				.Returns(new AppSettings { Ocr = new AppSettings.OcrSettings { Engine = OcrEngine.Tesseract } });
			files
				.Setup(files => files.GetPathSummary(It.IsAny<string>()))
				.Returns(new PathSummary { IsExistingDirectory = true });
			tesseract
				.Setup(tesseract => tesseract.IsReady())
				.Returns((false, "error"));
			var services = new ServiceCollection();
			services.AddSingleton(tesseract.Object);
			var provider = services.BuildServiceProvider();
			var sut = new OcrService(logger.Object, provider, settings.Object, files.Object);

			var actual = sut.Validate();

			Assert.Single(actual.Errors);
			Assert.Equal("Could not start Tesseract engine: error", actual.Errors.First());
		}

		[Fact]
		public async Task Execute_WhenEngineIsTesseract_ProcessFiles()
		{
			var logger = new Mock<ILogger<OcrService>>();
			var settings = new Mock<ISettingsService>();
			var files = new Mock<IFileRepository>();
			var tesseract = new Mock<ITesseractOcrRepository>();			
			settings
				.Setup(settings => settings.Settings)
				.Returns(new AppSettings { Ocr = new AppSettings.OcrSettings { Engine = OcrEngine.Tesseract } });
			files
				.Setup(files => files.GetFilesSummary(It.IsAny<string>(), SearchOption.AllDirectories, FileOrder.Alphabetical))
				.Returns(new List<FileSummary> { new FileSummary() });
			var services = new ServiceCollection();
			services.AddSingleton<ITesseractOcrRepository>(tesseract.Object);
			var provider = services.BuildServiceProvider();
			var sut = new OcrService(logger.Object, provider, settings.Object, files.Object);

			// Act
			await sut.Execute();

			// Assert
			tesseract.Verify(tesseract => tesseract.Process(It.IsAny<FileSummary>()), Times.Once);
		}
	}
}
