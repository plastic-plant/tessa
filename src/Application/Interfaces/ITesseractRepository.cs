using Tessa.Application.Models;

namespace Tessa.Application.Interfaces
{
	public interface ITesseractRepository
	{
		(bool ready, string? error) IsReady();
		FileSummary Process(FileSummary file);
	}
}
