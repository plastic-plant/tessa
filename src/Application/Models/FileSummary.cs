using System.Numerics;
using Tessa.Application.Enums;

namespace Tessa.Application.Models;

public class FileSummary: FileSummaryComparer
{
	/// <summary>
	/// The original path provided by the user. Can be a full path, a relative path or an incorrect path.
	/// </summary>
	public string? FilePathOriginal { get; set; }

	/// <summary>
	/// Verified existing absolute path, based on the original path provided by the user.
	/// </summary>
	public string? FilePathRooted { get; set; }
	public string? FileName { get; set; }
	public string? FileNameWithoutExtension { get; set; }
    public string? FileNameExtension { get; set; }
	public bool IsImage { get; set; }
	public float Confidence { get; set; }
	public string? FilePathResult { get; set; }
    public OcrProcessingStatus OcrProcessingStatus { get; set; }
}

public class FileSummaryComparer : IComparer<FileSummary>
{
	public int Compare(FileSummary a, FileSummary b)
	{
		return (a.FilePathRooted ?? "").CompareTo(b.FilePathRooted);
	}
}