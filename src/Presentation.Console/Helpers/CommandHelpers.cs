namespace Tessa.Presentation.Console.Helpers;

public static class CommandHelpers
{
	/// <summary>
	/// Updates the given setting if the new value is different from the default value.
	/// </summary>
	public static void ApplySetting<T>(T originalValue, T newValue, T defaultValue)
	{
		if (!EqualityComparer<T>.Default.Equals(newValue, defaultValue))
		{
			originalValue = newValue;
		}
	}
}
