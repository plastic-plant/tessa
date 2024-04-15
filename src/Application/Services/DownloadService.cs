using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using Tessa.Application.Interface;
using Tessa.Application.Interfaces;
using Tessa.Application.Models;

namespace Tessa.Application.Services
{
	public class DownloadService: IDownloadService
	{
		private readonly ILogger<DownloadService> _logger;
		private readonly IServiceProvider _services;
		private readonly ISettingsService _settings;

        public DownloadService(ILogger<DownloadService> logger, IServiceProvider services, ISettingsService settings)
        {
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_services = services ?? throw new ArgumentNullException(nameof(services));
			_settings = settings ?? throw new ArgumentNullException(nameof(settings));
		}

		public async Task DownloadFileAsync(string url, string destinationPath, Action<double> progressCallbackAction)
		{
			
			using (var httpClient = new HttpClient())
			{
				var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
				response.EnsureSuccessStatusCode();
				var contentLength = response.Content.Headers.ContentLength ?? -1;
				using (var stream = await response.Content.ReadAsStreamAsync())
				{
					var progress = new Progress<double>(progressCallbackAction);
					await SaveFile(stream, destinationPath, contentLength, progress);
				}
			}
		}
		private async Task SaveFile(Stream stream, string filePath, long contentLength, IProgress<double> progress)
		{
			const int bufferSize = 8192;
			var buffer = new byte[bufferSize];
			var bytesRead = 0L;
			var totalBytesRead = 0L;

			using (var fileStream = File.Create(filePath))
			{
				while ((bytesRead = await stream.ReadAsync(buffer, 0, bufferSize)) > 0)
				{
					await fileStream.WriteAsync(buffer, 0, (int)bytesRead);
					totalBytesRead += bytesRead;

					if (contentLength > 0)
					{
						var percentComplete = (double)totalBytesRead / contentLength * 100;
						progress.Report(percentComplete);
					}
				}
			}
		}
	}
}
