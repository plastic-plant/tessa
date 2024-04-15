using Tessa.Application.Models;

namespace Tessa.Application.Interface;

public interface ISettingsService
{
	AppRegistry Registry { get; }
	AppSettings Settings { get; }
	AppRegistry LoadRegistry();
	AppSettings LoadSettings(string? settingsPath);
	bool SaveSettings(string? settingsPath = null);
}
