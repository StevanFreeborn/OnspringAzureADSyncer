namespace OnspringAzureADSyncerTests.UnitTests;

public class SettingsTests
{
  private readonly Mock<IOptions<AppOptions>> _appOptionsMock;

  public SettingsTests()
  {
    _appOptionsMock = new Mock<IOptions<AppOptions>>();
  }

  [Fact]
  public void Settings_ItShouldHaveConstantPropertyForSettingsSection()
  {
    Settings.SettingsSection.Should().Be("Settings");
  }

  [Fact]
  public void Settings_ItShouldHaveDefaultGroupsFieldMappingsBasedOnConstantPropertiesOfAzureSettingsAndOnspringSettings()
  {
    Settings.DefaultGroupsFieldMappings.Should().BeEquivalentTo(new Dictionary<string, string>
    {
      {
        AzureSettings.GroupsNameKey,
        OnspringSettings.GroupsNameField
      },
      {
        AzureSettings.GroupsDescriptionKey,
        OnspringSettings.GroupsDescriptionField
      }
    });
  }

  [Fact]
  public void Settings_WhenConstructed_ItShouldHaveDefaultPropertyValues()
  {
    var filePath = Path.Combine(
      Directory.GetCurrentDirectory(),
      "testData",
      "testconfig.json"
    );

    _appOptionsMock
    .SetupGet(x => x.Value)
    .Returns(new AppOptions
    {
      ConfigFile = new FileInfo(filePath),
      LogLevel = LogEventLevel.Information
    });

    var settings = new Settings(_appOptionsMock.Object);
    settings.Azure.Should().NotBeNull();
    settings.Onspring.Should().NotBeNull();
    settings.GroupsFieldMappings.Should().NotBeNull();
    settings.UsersFieldMappings.Should().NotBeNull();
    settings.UsersFieldMappings.Should().BeEmpty();
    settings.GroupsFieldMappings.Should().BeEmpty();
  }

  [Fact]
  public void Settings_WhenInitialized_ItShouldBeAbleToSetItsProperties()
  {
    var filePath = Path.Combine(
      Directory.GetCurrentDirectory(),
      "testData",
      "testconfig.json"
    );

    _appOptionsMock
    .SetupGet(x => x.Value)
    .Returns(new AppOptions
    {
      ConfigFile = new FileInfo(filePath),
      LogLevel = LogEventLevel.Information
    });

    var azureSettings = new AzureSettings
    {
      ClientId = "ClientId",
      ClientSecret = "ClientSecret",
      TenantId = "TenantId"
    };

    var onspringSettings = new OnspringSettings
    {
      ApiKey = "ApiKey",
      BaseUrl = "https://api.onspring.com",
      UsersAppId = 1,
      GroupsAppId = 2
    };

    var groupFieldMappings = new Dictionary<string, int>
    {
      {
        "id",
        1
      },
      {
        "description",
        2
      }
    };

    var usersFieldMappings = new Dictionary<string, int>
    {
      {
        "id",
        1
      },
      {
        "description",
        2
      }
    };

    var settings = new Settings(_appOptionsMock.Object)
    {
      Azure = azureSettings,
      Onspring = onspringSettings,
      GroupsFieldMappings = groupFieldMappings,
      UsersFieldMappings = usersFieldMappings,
    };

    settings.Azure.Should().Be(azureSettings);
    settings.Onspring.Should().Be(onspringSettings);
    settings.GroupsFieldMappings.Should().BeEquivalentTo(groupFieldMappings);
    settings.UsersFieldMappings.Should().BeEquivalentTo(usersFieldMappings);
  }
}