namespace OnspringAzureADSyncer.Models;

public class Settings
{
  const string SettingsSection = "Settings";

  public AzureSettings Azure { get; init; } = new AzureSettings();
  public OnspringSettings Onspring { get; init; } = new OnspringSettings();
  public Dictionary<string, int> GroupFieldMappings { get; init; } = new Dictionary<string, int>();
  public Dictionary<string, int> UserFieldMappings { get; init; } = new Dictionary<string, int>();

  public Settings(IConfiguration configuration)
  {
    configuration.GetSection(SettingsSection).Bind(this);
  }
}