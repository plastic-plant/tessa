using Tessa.Application.Enums;

namespace Tessa.Application.Models;

public class FileSummary
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
	public string? FilePathResultOcr { get; set; }
	public string? FilePathResultLlm { get; set; }
	public OcrProcessingStatus OcrProcessingStatus { get; set; }

	public static FileSummary From(string filePathRooted)
	{
		var imageExtensions = new string[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff" };
		return new FileSummary()
		{
			FilePathRooted = filePathRooted,
			FileName = Path.GetFileName(filePathRooted),
			FileNameExtension = Path.GetExtension(filePathRooted),
			FileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePathRooted),
			IsImage = imageExtensions.Contains(Path.GetExtension(filePathRooted))
		};
	}
}