using Microsoft.Extensions.DependencyInjection;
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
		var services = new ServiceCollection();
		var provider = services.BuildServiceProvider();
		var settings = new Mock<ISettingsService>();
		settings.Setup(settings => settings.Settings).Returns(appSettings);
		var repository = new TesseractRepository(provider, settings.Object);
		var file = new FileSummary()
		{
			FilePathRooted = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "Examples", "example2.jpeg"),
			FileNameWithoutExtension = "example2",
			IsImage = true
		};
		var result = repository.Process(file);

		Assert.NotNull(result);
	}
}