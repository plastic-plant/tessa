using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tessa.Application.Interface;
using Tessa.Application.Interfaces;
using Tessa.Application.Models;
using Tessa.Application.Models.ProviderConfigs;
using Tessa.Infrastructure.Repositories;

namespace Infrastructure.Tests.Repositories;

public class LlamaRepositoryTests
{
	[Fact]
	public void IsReady_ModelUnknown_Returns()
	{
		var logger = new Mock<ILogger<LlamaRepository>>();
		var services = new Mock<IServiceProvider>();
		var settings = new Mock<ISettingsService>();
		settings
			.Setup(settings => settings.Settings)
			.Returns(new AppSettings()
			{
				Ocr = new AppSettings.OcrSettings() { SelectedProviderConfigName = "llama" },
				Llm = new AppSettings.LlmSettings() { ProviderConfigurations = new() { new ProviderConfigLlamaGguf() { Model = "path not given" } } }
			});
		var repository = new LlamaRepository(logger.Object, services.Object, settings.Object);

		var (ready, error) = repository.IsReady();

		Assert.False(ready);
		Assert.Contains("Could not open LLM:", error);
	}

	[Fact(Skip = "Rework")]
	public async Task ProcessAsync_Returns()
	{
		var logger = new Mock<ILogger<LlamaRepository>>();
		var services = new Mock<IServiceProvider>();
		var settings = new Mock<ISettingsService>();
		settings
			.Setup(settings => settings.Settings)
			.Returns(new AppSettings()
			{
				Ocr = new AppSettings.OcrSettings() { SelectedProviderConfigName = "llama" },
				Llm = new AppSettings.LlmSettings() { ProviderConfigurations = new() { new ProviderConfigLlamaGguf() { Model = "path not given" } } }
			});
		var repository = new LlamaRepository(logger.Object, services.Object, settings.Object);

		var file = new FileSummary
		{
			FilePathResultOcr = "test.txt",
			FileNameWithoutExtension = "test"
		};

		var result = await repository.ProcessAsync(file);

		Assert.NotNull(result);
		Assert.Equal("test.llm.txt", result.FilePathResultLlm);
	}

}
