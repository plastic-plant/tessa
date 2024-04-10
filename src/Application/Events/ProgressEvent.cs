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
		OcrProcessingStatus[] progressStates = [ OcrProcessingStatus.None, OcrProcessingStatus.Processing, OcrProcessingStatus.Optimizing, OcrProcessingStatus.Finished ];
		double[] progressStatesPosition = [ -1.0, -1.0, -0.5, 0.0 ];
		var currentProgressStatus = summary.CurrentFile?.OcrProcessingStatus ?? OcrProcessingStatus.Finished;
		int index = Array.IndexOf(progressStates, currentProgressStatus);
		double correctForProcessingState = index > -1 ? progressStatesPosition[index] : 0.0;

		double currentPosition = Summary.CurrentFilePosition + correctForProcessingState;
		double maximumPosition = Summary.Files.Count;

		double percentage = currentPosition / maximumPosition * 100;
		return (int)Math.Ceiling(percentage);
	}
}