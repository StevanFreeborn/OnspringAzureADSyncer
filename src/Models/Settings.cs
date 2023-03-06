namespace OnspringAzureADSyncer.Models;

public class Settings : ISettings
{
  internal const string SettingsSection = "Settings";
  internal static Dictionary<string, string> DefaultGroupsFieldMappings => new()
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

  public Settings(IOptions<AppOptions> options)
  {
    new ConfigurationBuilder()
    .AddJsonFile(
      options.Value.ConfigFile!.FullName,
      optional: false,
      reloadOnChange: true
    )
    .Build()
    .GetSection(SettingsSection)
    .Bind(this);
  }
}