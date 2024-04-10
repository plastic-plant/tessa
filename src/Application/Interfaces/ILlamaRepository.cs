using System.Threading.Tasks;
using Tessa.Application.Models;

namespace Tessa.Application.Interfaces;

public interface ILlamaRepository : IDisposable
{
	(bool ready, string? error) IsReady();
	Task<FileSummary> ProcessAsync(FileSummary file);
}
