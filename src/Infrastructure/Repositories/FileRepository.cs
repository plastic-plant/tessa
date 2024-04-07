using Tessa.Application.Enums;
using Tessa.Application.Interfaces;
using Tessa.Application.Models;

namespace Tessa.Infrastructure.Repositories
{
	public class FileRepository : IFileRepository
	{
		public PathSummary GetPathSummary(string givenPath)
		{
			var summary = new PathSummary()
			{
				PathOriginal = givenPath,
				PathRooted = Path.IsPathRooted(givenPath) ? givenPath : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, givenPath)
			};

			summary.IsExistingDirectory = Directory.Exists(summary.PathRooted);
			summary.IsExistingFile = File.Exists(summary.PathRooted);
			//summary.DirectoryFilesCount = summary.IsExistingDirectory ? Directory.GetFiles(summary.PathRooted).Length : 0;
			return summary;
		}

		public IEnumerable<FileSummary> GetFilesSummary(string path, SearchOption search = SearchOption.AllDirectories, FileOrder order = FileOrder.Alphabetical)
		{
			var list = new List<FileSummary>();
			var imageExtensions = new string[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff" };

			var summary = GetPathSummary(path);
			if (summary.IsExistingDirectory)
			{
				string[] filenames = Directory.GetFiles(summary.PathRooted!, "*.*", search);
				var ordered = order switch
				{
					FileOrder.Alphabetical => filenames.OrderBy(filename => filename).ToList(),
					FileOrder.ReverseAlphabetical => filenames.OrderByDescending(filename => filename).ToList(),
					FileOrder.CreationDate => filenames.OrderBy(filename => File.GetCreationTime(filename)).ToList(),
					FileOrder.ReverseCreationDate => filenames.OrderByDescending(filename => File.GetCreationTime(filename)).ToList(),
					FileOrder.ModifiedDate => filenames.OrderBy(filename => File.GetLastWriteTime(filename)).ToList(),
					FileOrder.ReverseModifiedDate => filenames.OrderByDescending(filename => File.GetLastWriteTime(filename)).ToList(),
					FileOrder.None => filenames.ToList(),
					_ => filenames.ToList()
				};


				list.AddRange(ordered.Select(filename => new FileSummary()
				{
					FilePathRooted = filename,
					FileName = Path.GetFileName(filename),
					FileNameExtension = Path.GetExtension(filename),
					FileNameWithoutExtension = Path.GetFileNameWithoutExtension(filename),
					IsImage = imageExtensions.Contains(Path.GetExtension(filename))
				}));
			}
			else
			{
				list.Add(new FileSummary()
				{
					FilePathRooted = summary.PathRooted,
					FileName = Path.GetFileName(summary.PathRooted),
					FileNameExtension = Path.GetExtension(summary.PathRooted),
					FileNameWithoutExtension = Path.GetFileName(summary.PathRooted),
					IsImage = imageExtensions.Contains(Path.GetExtension(summary.PathRooted))
				});
			}
			return list;
		}
	}
}
