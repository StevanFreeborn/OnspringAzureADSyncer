namespace OnspringAzureADSyncerTests.UnitTests;

public class ProgramTests : IDisposable
{
  public void Dispose()
  {
    Syncer.Start = Syncer.StartUp;
  }

  [Fact]
  public async Task Main_WhenCalledWithoutConfigOption_ShouldReturnNonZeroValue()
  {
    var args = Array.Empty<string>();
    var result = await Program.Main(args);
    result.Should().NotBe(0);
  }

  [Fact]
  public async Task Main_WhenCalledWithoutConfigOptionButWithLogLevelOption_ShouldReturnNonZeroValue()
  {
    var args = new[] { "--log-level", "Debug" };
    var result = await Program.Main(args);
    result.Should().NotBe(0);
  }

  [Fact]
  public async Task Main_WhenCalledWithConfigOption_ShouldReturnZeroValue()
  {
    Syncer.Start = (builder, options) => Task.FromResult(0);
    var args = new[] { "--config", "config.json" };
    var result = await Program.Main(args);
    result.Should().Be(0);
  }

  [Fact]
  public async Task Main_WhenCalledWithConfigOptionAndLogLevelOption_ShouldReturnZeroValue()
  {
    Syncer.Start = (builder, options) => Task.FromResult(0);
    var args = new[] { "--config", "config.json", "--log-level", "Debug" };
    var result = await Program.Main(args);
    result.Should().Be(0);
  }
}