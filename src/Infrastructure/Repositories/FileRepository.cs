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

		/// <summary>
		/// Gets a summary of files in given path. Path can be a directory or a single file.
		/// By default orders alphabatically and searches subdirectories.
		/// </summary>
		public SortedSet<FileSummary> GetFilesSummary(string path, SearchOption search = SearchOption.AllDirectories, FileOrder order = FileOrder.Alphabetical)
		{
			var list = new SortedSet<FileSummary>();			

			var summary = GetPathSummary(path);
			if (summary.IsExistingDirectory)
			{
				// Input path is a directory, let's gobble glob and sort.
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

				ordered.ForEach(filename => list.Add(AsFileSummary(filename)));
			}
			else
			{
				// Input path is a single file.
				list.Add(AsFileSummary(summary.PathRooted!));
			}

			return list;
		}

		private static FileSummary AsFileSummary(string filePathRooted)
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
}
