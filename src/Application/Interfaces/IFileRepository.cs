using Tessa.Application.Enums;
using Tessa.Application.Models;

namespace Tessa.Application.Interfaces;

public interface IFileRepository
{
	PathSummary GetPathSummary(string givenPath);
	List<FileSummary> GetFilesSummary(string path, SearchOption search = SearchOption.AllDirectories, FileOrder order = FileOrder.Alphabetical);
}
