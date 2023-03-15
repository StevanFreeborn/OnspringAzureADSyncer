namespace OnspringAzureADSyncerTests.UnitTests;

public class OnspringSettingsTests
{
  [Fact]
  public void OnspringSettings_ItShouldHaveProperConstantPropertyValues()
  {
    OnspringSettings.GroupsNameField.Should().Be("Name");
    OnspringSettings.GroupsDescriptionField.Should().Be("Description");
  }

  [Fact]
  public void OnspringSettings_WhenConstructed_ItShouldHaveDefaultPropertyValues()
  {
    var settings = new OnspringSettings();
    settings.ApiKey.Should().BeEmpty();
    settings.BaseUrl.Should().BeEmpty();
    settings.UsersAppId.Should().Be(0);
    settings.GroupsAppId.Should().Be(0);
  }

  [Fact]
  public void OnspringSettings_WhenInitialized_ItShouldBeAbleToSetItsProperties()
  {
    var settings = new OnspringSettings
    {
      ApiKey = "ApiKey",
      BaseUrl = "https://api.onspring.com",
      UsersAppId = 1,
      GroupsAppId = 2
    };

    settings.ApiKey.Should().Be("ApiKey");
    settings.BaseUrl.Should().Be("https://api.onspring.com");
    settings.UsersAppId.Should().Be(1);
    settings.GroupsAppId.Should().Be(2);
  }
}