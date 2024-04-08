namespace Tessa.Application.Models;

public class PathSummary
{
	/// <summary>
	/// The original path provided by the user. Can be a full path, a relative path or an incorrect path.
	/// </summary>
        public string? PathOriginal { get; set; }

	/// <summary>
	/// Verified existing absolute path, based on the original path provided by the user.
	/// </summary>
	public string? PathRooted { get; set; }

	public bool IsExistingDirectory { get; set; }
	public bool IsExistingFile { get; set; }
}
