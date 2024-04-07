using Tessa.Application.Models;

namespace Tessa.Application.Interfaces
{
	public interface ITesseractRepository : IDisposable
	{
		(string? version, string? error) IsReady();
		FileSummary Process(FileSummary file);
	}
}
