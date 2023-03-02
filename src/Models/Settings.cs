namespace OnspringAzureADSyncer.Models;

public class Settings
{
  public AzureSettings? Azure { get; init; }
  public OnspringSettings? Onspring { get; init; }
}