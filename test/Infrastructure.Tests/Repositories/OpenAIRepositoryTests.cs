using Microsoft.Extensions.Logging;
using Moq;
using Tessa.Application.Interface;
using Tessa.Application.Models;
using Tessa.Application.Models.ProviderConfigs;
using Tessa.Infrastructure.Repositories;

namespace Infrastructure.Tests.Repositories
{
	public class OpenAIRepositoryTests
	{
		[Fact(Skip = "Rework")]
		public async Task IsReadyAsync_WhenCalled_ReturnsReady()
		{
			// Arrange
			var logger = new Mock<ILogger<OpenAIRepository>>();
			var services = new Mock<IServiceProvider>();
			var settings = new Mock<ISettingsService>();
			settings
			.Setup(settings => settings.Settings)
			.Returns(new AppSettings()
			{
				Ocr = new AppSettings.OcrSettings() { SelectedProviderConfigName = "openai" },
				Llm = new AppSettings.LlmSettings() { ProviderConfigurations = new() { new ProviderConfigOpenAI() } }
			});
			var openAIRepository = new OpenAIRepository(logger.Object, services.Object, settings.Object);

			// Act
			var result = await openAIRepository.IsReadyAsync();

			// Assert
			Assert.True(result.ready);
		}
	}
}
