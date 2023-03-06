namespace OnspringAzureADSyncerTests.UnitTests;

public class AppOptionTests
{
  [Fact]
  public void AppOptionTests_WhenInitialized_ItShouldHavePropertiesThatCanBeSetAndGot()
  {
    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "test.json");

    var appOptions = new AppOptions
    {
      ConfigFile = new FileInfo(filePath),
      LogLevel = LogEventLevel.Debug
    };
    appOptions.ConfigFile.Should().NotBeNull();
    appOptions.ConfigFile.FullName.Should().Be(filePath);
    appOptions.LogLevel.Should().Be(LogEventLevel.Debug);
  }
}