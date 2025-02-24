namespace OnspringAzureADSyncer.Models;

public class Settings : ISettings
{
  internal const string SettingsSection = "Settings";
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

  public static Dictionary<string, string?> DefaultUsersFieldMappings => new()
  {
    {
      AzureSettings.UsersIdPropertyKey,
      null
    },
    {
      AzureSettings.UsersUsernameKey,
      OnspringSettings.UsersUsernameField
    },
    {
      AzureSettings.UsersFirstNameKey,
      OnspringSettings.UsersFirstNameField
    },
    {
      AzureSettings.UsersLastNameKey,
      OnspringSettings.UsersLastNameField
    },
    {
      AzureSettings.UsersEmailKey,
      OnspringSettings.UsersEmailField
    },
    {
      AzureSettings.UsersStatusKey,
      OnspringSettings.UsersStatusField
    },
    {
      AzureSettings.UsersGroupsKey,
      OnspringSettings.UsersGroupsField
    }
  };

  public AzureSettings Azure { get; init; } = new AzureSettings();
  public OnspringSettings Onspring { get; init; } = new OnspringSettings();
  public Dictionary<int, string> GroupsFieldMappings { get; init; } = [];
  public Dictionary<int, string> UsersFieldMappings { get; init; } = [];

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