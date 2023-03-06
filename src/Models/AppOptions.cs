namespace OnspringAzureADSyncer.Models;

public class AppOptions
{
  public FileInfo? ConfigFile { get; init; }
  public LogEventLevel LogLevel { get; init; }
}