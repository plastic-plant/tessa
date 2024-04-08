﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tessa.Application.Models;
using Tessa.Presentation.Console.Helpers;

namespace Application.Tests.Helpers
{
	public class CommandHelpersTests
	{
		[Fact]
		public void ApplySetting_WhenNewValueIsDifferentFromDefaultValue_UpdatesOriginalValueByReference()
		{
			var originalValue = "original";
			var newValue = "new";
			var defaultValue = "default";

			CommandHelpers.ApplySetting(ref originalValue, newValue, defaultValue);

			Assert.Equal(newValue, originalValue);
		}

		[Fact]
		public void ApplySetting_WhenNewValueIsDifferentFromDefaultValue_UpdatesOriginalPropertyWithoutReference()
		{
			var originalValue = new AppSettings()
			{
				Ocr = new AppSettings.OcrSettings()
				{
					InputPath = "original"
				}
			};
			var newValue = "new";
			var defaultValue = "default";

			CommandHelpers.ApplySetting(originalValue.Ocr.InputPath, newValue, defaultValue);

			Assert.Equal(newValue, originalValue.Ocr.InputPath);
		}

		[Theory]
		[InlineData("originalValue", "givenNewValue", "defaultValue", "givenNewValue")]
		[InlineData("originalValue", null, "defaultValue", "originalValue")]
		[InlineData("originalValue", "defaultValue", "defaultValue", "originalValue")]
		public void AppSettings_Override_Returns(string originalValue, string newValue, string defaultValue, string expectedValue)
		{
			var appSettings = new AppSettings()
			{
				Ocr = new AppSettings.OcrSettings()
				{
					LanguageTessdata = originalValue
				}
			};

			CommandHelpers.ApplySetting(originalValue, newValue, defaultValue);

			Assert.Equal(expectedValue, originalValue);
		}
	}
}
