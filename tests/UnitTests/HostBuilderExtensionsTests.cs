namespace OnspringAzureADSyncerTests.UnitTests;

public class HostBuilderExtensionsTests
{
  private readonly Mock<ISettings> _settingsMock;
  private readonly Mock<ILogger> _loggerMock;
  private readonly Mock<IAzureGroupDestructuringPolicy> _azureGroupDestructMock;
  private readonly Mock<IAzureUserDestructuringPolicy> _azureUserDestructMock;

  public HostBuilderExtensionsTests()
  {
    _settingsMock = new Mock<ISettings>();
    _loggerMock = new Mock<ILogger>();
    _azureGroupDestructMock = new Mock<IAzureGroupDestructuringPolicy>();
    _azureUserDestructMock = new Mock<IAzureUserDestructuringPolicy>();
  }

  [Fact]
  public void AddGraphClient_WhenCalledWithTenantIdClientIdAndClientSecret_ItShouldAddGraphServiceClientToServices()
  {
    _settingsMock
    .SetupGet(s => s.Azure)
    .Returns(
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

  [Fact]
  public void AddGraphClient_WhenCalledWithNoAzureSettings_ItShouldThrowException()
  {
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

    Action action = () => testHostBuilder.Build(); ;

    action.Should().Throw<Exception>();
  }

  [Fact]
  public void AddOnspringClient_WhenCalledWithApiKeyAndBaseUrl_ItShouldAddOnspringClientToServices()
  {
    _settingsMock
    .SetupGet(s => s.Onspring)
    .Returns(
      new OnspringSettings
      {
        ApiKey = "ApiKey",
        BaseUrl = "https://api.onspring.com"
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
    .AddOnspringClient();

    var testHost = testHostBuilder.Build();
    var onspringClient = testHost.Services.GetRequiredService<IOnspringClient>();

    onspringClient.Should().NotBeNull();
  }

  [Fact]
  public void AddOnspringClient_WhenCalledWithNoOnspringSettings_ItShouldThrowException()
  {
    var testHostBuilder = Host
    .CreateDefaultBuilder()
    .ConfigureServices(
      (context, services) =>
      {
        services.AddSingleton(_loggerMock.Object);
        services.AddSingleton(_settingsMock.Object);
      }
    )
    .AddOnspringClient();

    Action action = () => testHostBuilder.Build(); ;

    action.Should().Throw<Exception>();
  }

  [Theory]
  [InlineData(null, "ApiKey")]
  [InlineData("https://api.onspring.com", null)]
  [InlineData(null, null)]
  [InlineData("", "ApiKey")]
  [InlineData("https://api.onspring.com", "")]
  [InlineData("", "")]
  [InlineData(" ", "ApiKey")]
  [InlineData("https://api.onspring.com", " ")]
  [InlineData(" ", " ")]
  [InlineData("//api.onspring", "ApiKey")]
  public void AddOnspringClient_WhenCalledWithImproperSettings_ItShouldThrowException(
    string? baseUrl,
    string? apiKey
  )
  {
    _settingsMock
    .SetupGet(s => s.Onspring)
    .Returns(
      new OnspringSettings
      {
        BaseUrl = baseUrl!,
        ApiKey = apiKey!
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
    .AddOnspringClient();

    Action action = () => testHostBuilder.Build(); ;

    action.Should().Throw<Exception>();
  }

  [Fact]
  public void AddSerilog_WhenCalledWithLogger_ItShouldAddLoggerToServices()
  {
    var testHostBuilder = Host
    .CreateDefaultBuilder()
    .ConfigureServices(
      (context, services) =>
      {
        services.AddSingleton(_azureUserDestructMock.Object);
        services.AddSingleton(_azureGroupDestructMock.Object);
        services.AddSingleton(_loggerMock.Object);
        services.AddSingleton(_settingsMock.Object);
      }
    )
    .AddSerilog();

    var testHost = testHostBuilder.Build();
    var logger = testHost.Services.GetRequiredService<ILogger>();

    logger.Should().NotBeNull();
  }
}