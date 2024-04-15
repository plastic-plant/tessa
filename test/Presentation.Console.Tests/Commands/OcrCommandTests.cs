using Microsoft.Extensions.Logging;
using Moq;
using Spectre.Console.Cli;
using Tessa.Application.Interface;
using Tessa.Application.Interfaces;
using Tessa.Application.Models;
using Tessa.Presentation.Console.Commands;

namespace Presentation.Console.Tests.Commands;

public class OcrCommandTests
{
	OcrCommand.Settings _defaultSettings = new()
	{
		SettingsPath = AppSettings.Defaults.SettingsPath,
		InputPath = AppSettings.OcrSettings.Defaults.InputPath,
		OutputPath = AppSettings.OcrSettings.Defaults.OutputPath,
		TessdataLanguage = AppSettings.OcrSettings.Defaults.TessdataLanguage
	};

	[Fact]
    public void Validate_WithoutErrors_Returns()
    {
		var givenNoErrors = new List<string>();
		var logger = new Mock<ILogger<OcrCommand>>();
		var settingsService = new Mock<ISettingsService>();
		settingsService
			.Setup(settings => settings.LoadSettings(It.IsAny<string>()))
			.Returns(new AppSettings() { Errors = givenNoErrors });
		var ocrService = new Mock<IOcrService>();
		ocrService
			.Setup(ocr => ocr.Validate())
			.Returns(new OcrSummary() { Errors = givenNoErrors });
		var command = new OcrCommand(logger.Object, settingsService.Object, ocrService.Object);
		var remaining = new Mock<IRemainingArguments>();
		var context = new CommandContext(remaining.Object, "ocr", null);

		var actual = command.Validate(context, _defaultSettings);

		Assert.True(actual.Successful);
	}

	[Fact]
	public void Validate_WithErrors_Returns()
	{
		var givenSomeErrors = new List<string>() { "Some validation errors" };
		var logger = new Mock<ILogger<OcrCommand>>();
		logger
			.Setup(logger => logger.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<object>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception?, string>>()));
		var settingsService = new Mock<ISettingsService>();
		settingsService
			.Setup(settings => settings.LoadSettings(It.IsAny<string>()))
			.Returns(new AppSettings() { Errors = givenSomeErrors });
		var ocrService = new Mock<IOcrService>();
		ocrService
			.Setup(ocr => ocr.Validate())
			.Returns(new OcrSummary() { Errors = givenSomeErrors });
		var command = new OcrCommand(logger.Object, settingsService.Object, ocrService.Object);
		var remaining = new Mock<IRemainingArguments>();
		var context = new CommandContext(remaining.Object, "ocr", null);

		var actual = command.Validate(context, _defaultSettings);

		Assert.False(actual.Successful);
		logger.Verify(
			action => action.Log(
				It.Is<LogLevel>(level => level == LogLevel.Error),
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((message, type) => message.ToString()!.Contains(givenSomeErrors.First())),
				It.IsAny<Exception>(),
				It.Is<Func<It.IsAnyType, Exception?, string>>((formatter, type) => true)),
			Times.Once);
	}

}