using System.Text.Json;
using System.Text.Json.Serialization;
using Tessa.Application.Interface;
using Tessa.Application.Models;

namespace Tessa.Application.Services;

public class SettingsService : ISettingsService
{
	public AppRegistry Registry { get; private set; } = new();
	public AppSettings Settings { get; private set; } = new();

	private JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web)
	{
		WriteIndented = true,
		PropertyNameCaseInsensitive = true,
		AllowTrailingCommas = true,
		NumberHandling = JsonNumberHandling.AllowReadingFromString,
		Converters = { new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower) },
		ReadCommentHandling = JsonCommentHandling.Skip,
		PreferredObjectCreationHandling = JsonObjectCreationHandling.Populate,
	};

	public AppRegistry LoadRegistry()
	{
		var filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessa.registry.json");
		try
		{
			if (File.Exists(filename))
			{
				var content = File.ReadAllText(filename);
				if (!string.IsNullOrWhiteSpace(content))
				{
					Registry = JsonSerializer.Deserialize<AppRegistry>(content, _serializerOptions)!;
				}
			}
			else
			{
				Settings.Errors.Add($"Could not load registry from {filename}. The file name was matched with {AppDomain.CurrentDomain.BaseDirectory} but not found.");
			}
		}
		catch (JsonException e)
		{
			Settings.Errors.Add($"Could not load registry from {filename}. There's an error in the formatting of the JSON content. Please run tessa config to fix. {e.Message}");
		}
		catch (Exception)
		{
			Settings.Errors.Add($"Could not load registry from {filename}. The file name is not found.");
		}

		return Registry;
	}	

	public AppSettings LoadSettings(string? settingsPath = null)
	{
		var filename = settingsPath ?? Settings.SettingsPath;
		try
		{
			if (!Path.IsPathRooted(filename))
			{
				filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);
			}

			if (File.Exists(filename))
			{
				var content = File.ReadAllText(filename);
				if (!string.IsNullOrWhiteSpace(content))
				{
					Settings = JsonSerializer.Deserialize<AppSettings>(content, _serializerOptions)!;
					Settings.SettingsPath = filename;
				}
			}
			else
			{
				Settings.Errors.Add($"Could not load settings from {filename}. Please run tessa config to create a new tessa.config.json.");
			}
		}
		catch (ArgumentException)
		{
			Settings.Errors.Add($"Could not load settings from {filename}. The file name was matched with {AppDomain.CurrentDomain.BaseDirectory} but not found.");
		}
		catch (JsonException e)
		{
			Settings.Errors.Add($"Could not load settings from {filename}. There's an error in the formatting of the JSON content. Please run tessa config to fix. {e.Message}");
		}
		catch (Exception)
		{
			Settings.Errors.Add($"Could not load settings from {filename}. The file name is not found.");
		}

		return Settings;
	}

	public bool SaveSettings(string? settingsPath = null)
	{
		try
		{
			var json = JsonSerializer.Serialize(Settings, _serializerOptions);
			File.WriteAllText(settingsPath ?? Settings.SettingsPath, json);
			return true;
		}
		catch (Exception e)
		{
			Settings.Errors.Add($"Could not save settings to {settingsPath}: {e.Message}");
			return false;
		}
	}
}
