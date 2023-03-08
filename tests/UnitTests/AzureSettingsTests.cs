namespace OnspringAzureADSyncerTests.UnitTests;

public class AzureSettingsTests
{
  [Fact]
  public void AzureSettings_ItShouldHaveProperConstantPropertyValues()
  {
    AzureSettings.GroupsNameKey.Should().Be("id");
    AzureSettings.GroupsDescriptionKey.Should().Be("description");
  }

  [Fact]
  public void AzureSettings_WhenConstructed_ItShouldHaveDefaultPropertyValues()
  {
    var settings = new AzureSettings();
    settings.TenantId.Should().BeEmpty();
    settings.ClientId.Should().BeEmpty();
    settings.ClientSecret.Should().BeEmpty();
  }

  [Fact]
  public void AzureSettings_WhenInitialized_ItShouldBeAbleToSetItsProperties()
  {
    var settings = new AzureSettings
    {
      TenantId = "tenantId",
      ClientId = "clientId",
      ClientSecret = "clientSecret"
    };

    settings.TenantId.Should().Be("tenantId");
    settings.ClientId.Should().Be("clientId");
    settings.ClientSecret.Should().Be("clientSecret");
  }
}