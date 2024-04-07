using Tessa.Application.Models;
using Tessa.Infrastructure.Extensions;

namespace Application.Tests.Extensions;

public class AppSettingsExtensionsTests
{
    [Theory]
	[InlineData("defaultValue", "originalValue", "givenNewValue", "givenNewValue")]
	[InlineData("defaultValue", "originalValue", null, "originalValue")]
	[InlineData("defaultValue", "originalValue", "defaultValue", "originalValue")]
	public void AppSettings_Override_Returns(string defaultValue, string originalValue, string givenValue, string expectedValue)
    {
        var appSettings = new AppSettings()
        {
            Ocr = new AppSettings.OcrSettings()
            {
                LanguageTessdata = originalValue
			}
        };

        var actual = appSettings.Override(property => property.Ocr.LanguageTessdata, givenValue, defaultValue);

        Assert.Equal(expectedValue, actual.Ocr.LanguageTessdata);
    }
}
