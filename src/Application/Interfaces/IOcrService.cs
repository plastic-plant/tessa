using Tessa.Application.Models;

namespace Tessa.Application.Interfaces;

public interface IOcrService
{
	OcrSummary Validate();
	Task<OcrSummary> Execute();
	Task<OcrSummary> Cancel();
	Task<OcrSummary> Pause();
	Task<OcrSummary> Stop();
	Task<OcrSummary> Verify();
}
