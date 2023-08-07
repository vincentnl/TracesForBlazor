using FluentAssertions;
using TracesForBlazor;

namespace TracerForBlazorTests;

public class OptionsTests
{
    [Theory]
    [InlineData("http://localhost:4318", true)]
    [InlineData("http://localhost4318", false)]
    [InlineData("http:/localhost:4318", false)]
    [InlineData("localhost:4318", false)]
    [InlineData("ftp://localhost:4318", false)]
    public void InvalidUrl_Triggers_Exception(string url, bool outcome)
    {
        TracerForBlazorOptions options = new()
        {
            Url = url
        };
        Action callToVerifyOptions = () => ConfigureService.VerifyOptions(options);
        if (outcome)
        {
            callToVerifyOptions.Should().NotThrow();
        }
        else
        {
            callToVerifyOptions.Should().Throw<ArgumentException>();
        }
    }
}