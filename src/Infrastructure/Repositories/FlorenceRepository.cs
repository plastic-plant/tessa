using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Microsoft.Extensions.Logging;
using System.Text;
using Tessa.Application.Enums;
using Tessa.Application.Interface;
using Tessa.Application.Interfaces;
using Tessa.Application.Models;
using Tesseract;
using Florence2;
using System.Threading.Tasks;

namespace Tessa.Infrastructure.Tesseract;

public class FlorenceRepository : IFlorenceOcrRepository
{
	private readonly ILogger<FlorenceRepository> _logger;
	private readonly IServiceProvider _services;
	private readonly AppSettings.OcrSettings _settings;
	private readonly string _florenceModelPath;

	public FlorenceRepository(ILogger<FlorenceRepository> logger, IServiceProvider services, ISettingsService settings)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_services = services ?? throw new ArgumentNullException(nameof(services));
		_settings = settings?.Settings?.Ocr ?? throw new ArgumentNullException(nameof(settings));
		_florenceModelPath = Path.Combine(_settings.ModelsPath, "florence-2");
	}

	public (bool ready, string? error) IsReady()
	{
		var decodermodel = Path.Combine(_florenceModelPath, "decoder_model_merged.onnx");
		if (!File.Exists(decodermodel))
		{
			return (false, $"Could not find Florence model at path: {_florenceModelPath}. Type: tessa download llm florence-2");
		}

		return (true, null);
	}

	public FileSummary Process(FileSummary file)
	{
		var modelSource = new FlorenceModelDownloader(Path.Combine(_settings.ModelsPath, "florence-2"));
		var model = new Florence2Model(modelSource);
		try
		{
			var text = new List<string>();
			if (file.IsImage)
			{
				using var imageStream = File.OpenRead(file.FilePathRooted);
				string prompt = "";
				var results = model.Run(TaskTypes.OCR_WITH_REGION, imageStream, prompt, CancellationToken.None);
				foreach (var result in results)
				{
					foreach (var box in result.OCRBBox)
					{
						text.Add(box.Text);
					}
				}
				file.FilePathResultOcr = Path.Combine(_settings.OutputPath, $"{file.FileNameWithoutExtension!}.ocr.txt");
				File.WriteAllText(file.FilePathResultOcr, string.Join(' ', text));
				_logger.LogDebug($"Florence processed {file.FileName} with confidence {file.Confidence}");
			}
			else
			{
				_logger.LogInformation($"Florence did not process {file.FileName}.");
			}
		}
		catch (Exception ex)
		{
			file.OcrProcessingStatus = OcrProcessingStatus.Failed;
			var failMessage = $"Florence failed to process {file.FileName}: {ex.Message}";
			_logger.LogError(ex, failMessage);
			file.Errors.Add(failMessage);
		}

		return file;
	}

	public async Task DownloadModelsAsync(Action<double> progressPercentageCallback)
	{
		var modelSource = new FlorenceModelDownloader(Path.Combine(_settings.ModelsPath, "florence-2"));
		var onStatusUpdate = new Action<IStatus>(status => progressPercentageCallback.Invoke(status.Progress * 100));
		await modelSource.DownloadModelsAsync(onStatusUpdate, _logger);
	}
}

