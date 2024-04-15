namespace Tessa.Application.Interfaces;

public interface IDownloadService
{
	Task DownloadFileAsync(string url, string destinationPath, Action<double> progressCallbackAction);
}
