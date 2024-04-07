using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
			var services = new Mock<IServiceProvider>();
			var settings = new Mock<ISettingsService>();
			var files = new Mock<IFileRepository>();
			var sut = new OcrService(services.Object, settings.Object, files.Object);
			settings
				.Setup(settings => settings.Settings)
				.Returns(new AppSettings { Ocr = new AppSettings.OcrSettings { InputPath = "path" } });
			files
				.Setup(files => files.GetPathSummary("path"))
				.Returns(new PathSummary { IsExistingDirectory = false, IsExistingFile = false });

			var actual = sut.Validate();

			Assert.Single(actual.Errors);
			Assert.Equal("Could not open folder or file path.", actual.Errors.First());
		}

		[Fact]
		public void Validate_WhenEngineIsTesseractAndRepositoryIsNotReady_Error()
		{
			var services = new Mock<IServiceProvider>();
			var settings = new Mock<ISettingsService>();
			var files = new Mock<IFileRepository>();
			var tesseract = new Mock<ITesseractRepository>();
			var sut = new OcrService(services.Object, settings.Object, files.Object);
			settings
				.Setup(settings => settings.Settings)
				.Returns(new AppSettings { Ocr = new AppSettings.OcrSettings { Engine = OcrEngine.Tesseract } });
			files
				.Setup(files => files.GetPathSummary(It.IsAny<string>()))
				.Returns(new PathSummary { IsExistingDirectory = true });
			services
				.Setup(services => services.GetService<ITesseractRepository>())
				.Returns(tesseract.Object);
			tesseract
				.Setup(tesseract => tesseract.IsReady())
				.Returns((null, "error"));

			var actual = sut.Validate();

			Assert.Single(actual.Errors);
			Assert.Equal("Could not start Tesseract engine: error", actual.Errors.First());
		}

		[Fact]
		public async Task Execute_WhenEngineIsTesseract_ProcessFiles()
		{
			var services = new Mock<IServiceProvider>();
			var settings = new Mock<ISettingsService>();
			var files = new Mock<IFileRepository>();
			var tesseract = new Mock<ITesseractRepository>();
			var sut = new OcrService(services.Object, settings.Object, files.Object);
			settings
				.Setup(settings => settings.Settings)
				.Returns(new AppSettings { Ocr = new AppSettings.OcrSettings { Engine = OcrEngine.Tesseract } });
			files
				.Setup(files => files.GetFilesSummary(It.IsAny<string>(), SearchOption.AllDirectories, FileOrder.Alphabetical))
				.Returns(new List<FileSummary> { new FileSummary() });
			services
				.Setup(services => services.GetRequiredService<ITesseractRepository>())
				.Returns(tesseract.Object);

			// Act
			await sut.Execute();

			// Assert
			tesseract.Verify(tesseract => tesseract.Process(It.IsAny<FileSummary>()), Times.Once);
		}
	}
}
