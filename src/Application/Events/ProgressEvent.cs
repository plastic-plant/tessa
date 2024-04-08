using Tessa.Application.Enums;
using Tessa.Application.Models;

namespace Tessa.Application.Events;

public delegate void ProgressEventHandler(object sender, ProgressEventArgs e);

public class ProgressEventArgs : EventArgs
{
    public OcrSummary Summary { get; private set; }
    public int ProgressPercentage { get; private set; }

	public ProgressEventArgs(OcrSummary summary)
	{
		Summary = summary;
		ProgressPercentage = GetProgressPercentage(summary);
	}

	private int GetProgressPercentage(OcrSummary summary)
	{
		int correctForProcessingState = OcrProcessingStatus.Finished == summary.CurrentFile?.OcrProcessingStatus ? 0 : -1;
		int currentPosition = Summary.CurrentFilePosition + correctForProcessingState;
		double percentage = (double)currentPosition / (double)Summary.Files.Count;
		return (int)Math.Ceiling(percentage * 100);
	}
}