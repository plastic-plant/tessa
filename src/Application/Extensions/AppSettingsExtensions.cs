using System.Linq.Expressions;
using Tessa.Application.Models;

namespace Tessa.Infrastructure.Extensions;

public static class AppSettingsExtensions
{
	/// <summary>
	/// Takes the current AppSettings object and sets the given property. Property can be set for nested objects
	/// (e.g. property.Level1.Level2.Level3.LastPropertyName). Ignores when matching a optional given default value.
	/// 
	/// Example: appsettings.Override(property => property.Ocr.language, "eng")
	/// Example: appsettings.Override(property => property.Ocr.language, "nld", OcrSettings.IgnoreMatchingThisDefaultLanguage)
	/// </summary>
	public static AppSettings Override<TValue>(this AppSettings appSettings, Expression<Func<AppSettings, TValue>> propertySelector, TValue value, TValue? defaultValue = null) where TValue : class
	{
		var memberExpression = propertySelector.Body as MemberExpression;
		var propertyNames = new List<string>();
		while (memberExpression != null)
		{
			propertyNames.Add(memberExpression.Member.Name);
			memberExpression = memberExpression.Expression as MemberExpression;
		}

		propertyNames.Reverse();
		object? parentObject = appSettings;
		foreach (var propertyName in propertyNames)
		{
			var tryPropertyName = parentObject?.GetType().GetProperty(propertyName); // ?.GetValue(currentObject);
			if (tryPropertyName != null)
			{
				parentObject = tryPropertyName;
			}
		}
		
		var finalPropertyName = propertyNames.Last();
		var finalProperty = parentObject?.GetType().GetProperty(finalPropertyName);
		if (finalProperty != null && value != null && !value.Equals(defaultValue))
		{
			finalProperty?.SetValue(parentObject, value);

		}

		return appSettings;
	}

}
