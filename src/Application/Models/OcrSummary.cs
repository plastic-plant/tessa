namespace Tessa.Application.Models;

public class OcrSummary
{
	public List<string> Errors { get; set; } = new();
	public List<FileSummary> Files { get; set; } = new();
    public FileSummary?  CurrentFile { get; set; }
    public int CurrentFilePosition { get; set; }
    public Question Questions { get; set; } = Question.None;

    public enum Question
	{
		None,
		AllowDownloadTessdataLanguage,
		AllowDownloadLargeLanguageModel
	}
}
