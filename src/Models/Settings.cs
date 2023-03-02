namespace OnspringAzureADSyncer.Models;

public class Settings
{
  const string SettingsSection = "Settings";

  public AzureSettings Azure { get; init; } = new AzureSettings();
  public OnspringSettings Onspring { get; init; } = new OnspringSettings();

  public Settings(IConfiguration configuration)
  {
    configuration.GetSection(SettingsSection).Bind(this);
  }
}