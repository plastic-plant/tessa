using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tessa.Application.Models;

namespace Tessa.Application.Interfaces
{
	public interface ISettingsService : ISettingsService<AppSettings>;

	public interface ISettingsService<T> where T : class
	{
		T Settings { get; }
		T LoadAppSettingsFromFile(string? configPath);
		bool Save();
	}
}
