﻿using Tessa.Application.Models;

namespace Tessa.Application.Interface;

public interface ISettingsService : ISettingsService<AppSettings>;

public interface ISettingsService<T> where T : class
{
	T Settings { get; }
	T Load(string? settingsPath);
	bool Save(string settingsPath);
}
