using Tessa.Application.Models;

namespace Tessa.Application.Interface;

public interface ISettingsService : ISettingsService<AppSettings>;

public interface ISettingsService<T> where T : class
{
	AppRegistry Registry { get; }
	T Settings { get; }
	AppRegistry LoadRegistry();
	T LoadSettings(string? settingsPath);
	bool SaveSettings(string settingsPath);
}
