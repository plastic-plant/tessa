using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Reflection;
using Tessa.Application.Interface;
using Tessa.Application.Models;
using Tessa.Infrastructure.Tesseract;

namespace Tessa.Infrastructure.Tests.Repositories;

public class TesseractRepositoryTests
{

    [Fact]
	public void Process_Jpeg_Returns()
	{
		var appSettings = new AppSettings();
		appSettings.Ocr.OutputPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!);
		var logger = new Mock<ILogger<TesseractRepository>>();
		var services = new ServiceCollection();
		var provider = services.BuildServiceProvider();
		var settings = new Mock<ISettingsService>();
		settings.Setup(settings => settings.Settings).Returns(appSettings);
		var repository = new TesseractRepository(logger.Object, provider, settings.Object);
		var file = new FileSummary()
		{
			FilePathRooted = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "Examples", "example-2.jpeg"),
			FileNameWithoutExtension = "example-2",
			IsImage = true
		};
		var result = repository.Process(file);

		Assert.NotNull(result);
	}


	[Fact]
	public void Process_Pdf_Returns()
	{
		var appSettings = new AppSettings();
		appSettings.Ocr.OutputPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!);
		var logger = new Mock<ILogger<TesseractRepository>>();
		var services = new ServiceCollection();
		var provider = services.BuildServiceProvider();
		var settings = new Mock<ISettingsService>();
		settings.Setup(settings => settings.Settings).Returns(appSettings);
		var repository = new TesseractRepository(logger.Object, provider, settings.Object);
		var file = new FileSummary()
		{
			FilePathRooted = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "Examples", "example-3.pdf"),
			FileNameWithoutExtension = "example-3",
			IsPdf = true
		};
		var result = repository.Process(file);

		Assert.NotNull(result);
	}
}