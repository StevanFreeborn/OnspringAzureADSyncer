namespace OnspringAzureADSyncerTests.UnitTests;

public class HostBuilderExtensionsTests
{
  private readonly Mock<ISettings> _settingsMock;
  private readonly Mock<ILogger> _loggerMock;

  public HostBuilderExtensionsTests()
  {
    _settingsMock = new Mock<ISettings>();
    _loggerMock = new Mock<ILogger>();
  }

  [Fact]
  public void AddGraphClient_WhenCalledWithTenantIdClientIdAndClientSecret_ItShouldAddGraphServiceClientToServices()
  {
    _settingsMock.SetupGet(s => s.Azure).Returns(
      new AzureSettings
      {
        ClientId = "ClientId",
        ClientSecret = "ClientSecret",
        TenantId = "TenantId"
      }
    );

    var testHostBuilder = Host
    .CreateDefaultBuilder()
    .ConfigureServices(
      (context, services) =>
      {
        services.AddSingleton(_loggerMock.Object);
        services.AddSingleton(_settingsMock.Object);
      }
    )
    .AddGraphClient();

    var testHost = testHostBuilder.Build();
    var graphServiceClient = testHost.Services.GetRequiredService<GraphServiceClient>();

    graphServiceClient.Should().NotBeNull();
  }
}