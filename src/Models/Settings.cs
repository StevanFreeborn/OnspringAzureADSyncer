namespace OnspringAzureADSyncer.Models;

public class Settings
{
  const string SettingsSection = "Settings";

  public AzureSettings? Azure { get; set; }
  public OnspringSettings? Onspring { get; set; }

  public Settings(IConfiguration configuration)
  {
    configuration.GetSection(SettingsSection).Bind(this);
  }
}