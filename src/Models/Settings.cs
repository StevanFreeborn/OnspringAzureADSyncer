namespace OnspringAzureADSyncer.Models;

class Settings
{
  const string SettingsSection = "Settings";
  public static Dictionary<string, string> DefaultGroupsFieldMappings => new()
  {
    {
      AzureSettings.GroupsNameKey,
      OnspringSettings.GroupsNameField
    },
    {
      AzureSettings.GroupsDescriptionKey,
      OnspringSettings.GroupsDescriptionField
    }
  };

  public AzureSettings Azure { get; init; } = new AzureSettings();
  public OnspringSettings Onspring { get; init; } = new OnspringSettings();
  public Dictionary<string, int> GroupsFieldMappings { get; init; } = new Dictionary<string, int>();
  public Dictionary<string, int> UsersFieldMappings { get; init; } = new Dictionary<string, int>();

  public Settings(IConfiguration configuration)
  {
    configuration.GetSection(SettingsSection).Bind(this);
  }
}