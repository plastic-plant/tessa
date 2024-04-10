using Tessa.Application.Enums;
using Tessa.Application.Events;
using Tessa.Application.Models;

namespace Application.Tests.Events;

public class ProgressEventTests
{
	[Theory]
	[InlineData(1, 100, OcrProcessingStatus.None, 0)]
	[InlineData(1, 100, OcrProcessingStatus.Finished, 1)]
	[InlineData(2, 100, OcrProcessingStatus.None, 1)]
	[InlineData(2, 100, OcrProcessingStatus.Finished, 2)]
	[InlineData(50, 100, OcrProcessingStatus.Finished, 50)]
	[InlineData(100, 100, OcrProcessingStatus.None, 99)]
	[InlineData(100, 100, OcrProcessingStatus.Finished, 100)]
	[InlineData(1, 2, OcrProcessingStatus.None, 0)]
	[InlineData(1, 2, OcrProcessingStatus.Processing, 0)]
	[InlineData(1, 2, OcrProcessingStatus.Optimizing, 25)]
	[InlineData(1, 2, OcrProcessingStatus.Finished, 50)]
	[InlineData(1, 2, OcrProcessingStatus.Failed, 50)]
	[InlineData(1, 2, OcrProcessingStatus.Skipped, 50)]
	[InlineData(2, 2, OcrProcessingStatus.None, 50)]
	[InlineData(2, 2, OcrProcessingStatus.Processing, 50)]
	[InlineData(2, 2, OcrProcessingStatus.Optimizing, 75)]
	[InlineData(2, 2, OcrProcessingStatus.Finished, 100)]
	[InlineData(2, 2, OcrProcessingStatus.Failed, 100)]
	[InlineData(2, 2, OcrProcessingStatus.Skipped, 100)]

	public void ProgressEvent_ProgressPercentage(int currentItem, int totalItems, OcrProcessingStatus status, int expectedPercentage)
	{
		var summary = new OcrSummary()
		{
			CurrentFilePosition = currentItem
		};

		summary.CurrentFile = new FileSummary
		{
			OcrProcessingStatus = status
		};

		for (int i = 0; i < totalItems; i++)
		{
			summary.Files.Add(new FileSummary());
		}

		var actual = new ProgressEventArgs(summary);

		Assert.Equal(expectedPercentage, actual.ProgressPercentage);
	}
}
