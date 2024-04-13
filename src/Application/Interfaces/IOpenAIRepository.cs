using Tessa.Application.Models;

namespace Tessa.Application.Interfaces
{
	public interface IOpenAIRepository
	{
		Task<(bool ready, string? error)> IsReadyAsync();
		Task<FileSummary> ProcessAsync(FileSummary file);
	}
}
