using Microsoft.Extensions.Logging;
using Moq;
using Tessa.Application.Interface;
using Tessa.Infrastructure.Repositories;

namespace Infrastructure.Tests.Repositories
{
	public class OpenAIRepositoryTests
	{
		[Fact]
		public async Task IsReadyAsync_WhenCalled_ReturnsReady()
		{
			// Arrange
			var logger = new Mock<ILogger<OpenAIRepository>>();
			var services = new Mock<IServiceProvider>();
			var settings = new Mock<ISettingsService>();
			var openAIRepository = new OpenAIRepository(logger.Object, services.Object, settings.Object);

			// Act
			var result = await openAIRepository.IsReadyAsync();

			// Assert
			Assert.True(result.ready);
		}
	}
}
