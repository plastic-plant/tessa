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

namespace Tessa.Infrastructure.Tesseract;

public class TesseractRepository : ITesseractRepository
{
	private readonly ILogger<TesseractRepository> _logger;
	private readonly IServiceProvider _services;
	private readonly AppSettings.OcrSettings _settings;

	public TesseractRepository(ILogger<TesseractRepository> logger, IServiceProvider services, ISettingsService settings)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_services = services ?? throw new ArgumentNullException(nameof(services));
		_settings = settings?.Settings?.Ocr ?? throw new ArgumentNullException(nameof(settings));
	}

	public (bool ready, string? error) IsReady()
	{
		try
		{
			// Verify that the requested Tesseract models are available.
			foreach (var language in _settings.TessdataLanguage.Split("+"))
			{
				var tesseractmodel = Path.Combine(_settings.TessdataPath, $"{language}.traineddata");
				if (!File.Exists(tesseractmodel))
				{
					return (false, $"Could not find Tesseract model for language {language} at path: {tesseractmodel}. Type: tessa download tessdata {language}");
				}
			}

			// Verify that Tesseract can be called with the given parameters.
			using var engine = new TesseractEngine(_settings.TessdataPath, _settings.TessdataLanguage);
			return (!string.IsNullOrWhiteSpace(engine.Version), null);
		}
		catch (Exception ex)
		{
			return (false, ex.Message);
		}
	}

	public FileSummary Process(FileSummary file)
	{
		try
		{
			using var engine = new TesseractEngine(_settings.TessdataPath, _settings.TessdataLanguage, EngineMode.LstmOnly);
			var text = new StringBuilder();

			// Process image file.
			if (file.IsImage)
			{
				using var image = Pix.LoadFromFile(file.FilePathRooted);
				using var page = engine.Process(image);				
				file.Confidence = page.GetMeanConfidence();
				text.AppendLine(page.GetText());
			}

			// Process PDF document file.
			if (file.IsPdf)
			{
				using var reader = new PdfReader(file.FilePathRooted);
				using var document = new PdfDocument(reader);
				var parser = new PdfDocumentContentParser(document);
				var strategy = new LocationTextExtractionStrategy();
				for (int pageNum = 1; pageNum <= document.GetNumberOfPages(); pageNum++)
				{
					// Extract text from each page.
					parser.ProcessContent(pageNum, strategy);
					text.Append($" {strategy.GetResultantText()}");

					// Extract images from each page for OCR.
					var listener = new ImageRenderListener(engine, file, text);
					var canvas = new PdfCanvasProcessor(listener);
					PdfPage page = document.GetPage(pageNum);
					canvas.ProcessPageContent(page);
				}
				document.Close();
				file.Confidence = 1;
			}
			
			file.FilePathResultOcr = Path.Combine(_settings.OutputPath, $"{file.FileNameWithoutExtension!}.ocr.txt");
			File.WriteAllText(file.FilePathResultOcr, text.ToString());
			_logger.LogDebug($"Tesseract processed {file.FileName} with confidence {file.Confidence}");
		}
		catch (Exception ex)
		{
			file.OcrProcessingStatus = OcrProcessingStatus.Failed;
			var failMessage = $"Tesseract failed to process {file.FileName}: {ex.Message}";
			_logger.LogError(ex, failMessage);
			file.Errors.Add(failMessage);
		}

		return file;
	}
}

public class ImageRenderListener : IEventListener
{
	private TesseractEngine engine;
	private FileSummary file;
	private StringBuilder text;

	public ImageRenderListener(TesseractEngine engine, FileSummary file, StringBuilder text)
	{
		this.engine = engine ?? throw new ArgumentNullException(nameof(engine));
		this.file = file ?? throw new ArgumentNullException(nameof(file));
		this.text = text ?? throw new ArgumentNullException(nameof(text));
	}

	public void EventOccurred(IEventData data, EventType type)
	{
		if (type.Equals(EventType.RENDER_IMAGE))
		{
			ImageRenderInfo renderInfo = (ImageRenderInfo)data;
			try
			{
				// Extract image.
				byte[] imageBytes = renderInfo.GetImage().GetImageBytes();

				// Process image with Tesseract OCR.
				using var image = Pix.LoadFromMemory(imageBytes);
				using var page = engine.Process(image);
				file.Confidence = page.GetMeanConfidence();
				text.AppendLine(page.GetText());
			}
			catch (IOException ex)
			{
				file.Errors.Add($"Error extracting image from PDF: {ex.Message}");
			}
		}
	}

	public ICollection<EventType> GetSupportedEvents()
	{
		return new List<EventType> { EventType.RENDER_IMAGE };
	}
}