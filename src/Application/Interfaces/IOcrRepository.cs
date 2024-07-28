using System.Net.NetworkInformation;
using Tessa.Application.Models;

namespace Tessa.Application.Interfaces
{
	public interface IOcrRepository
	{
		(bool ready, string? error) IsReady();
		FileSummary Process(FileSummary file);
	}

	public interface ITesseractOcrRepository: IOcrRepository
	{
	}

	public interface IFlorenceOcrRepository : IOcrRepository
	{
		Task DownloadModelsAsync(Action<double> progressCallback);
	}
}
