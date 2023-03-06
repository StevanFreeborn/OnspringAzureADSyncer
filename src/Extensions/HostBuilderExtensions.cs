namespace OnspringAzureADSyncer.Extensions;

public static class HostBuilderExtensions
{
  public static IHostBuilder AddGraphClient(this IHostBuilder hostBuilder)
  {
    return hostBuilder
    .ConfigureServices(
      (context, services) =>
      {
        var settings = services.BuildServiceProvider().GetRequiredService<ISettings>();
        var logger = services.BuildServiceProvider().GetRequiredService<ILogger>();

        try
        {
          var graphServiceClient = new GraphServiceClient(
            new ClientSecretCredential(
              settings.Azure.TenantId,
              settings.Azure.ClientId,
              settings.Azure.ClientSecret,
              new TokenCredentialOptions
              {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
              }
            ),
            new[] { "https://graph.microsoft.com/.default" }
          );

          services.AddSingleton(graphServiceClient);
        }
        catch (Exception ex)
        {
          logger.Fatal(
            ex,
            "Unable to create Graph client: {Message}",
            ex.Message
          );
          throw;
        }
      }
    );
  }

  public static IHostBuilder AddOnspringClient(this IHostBuilder hostBuilder)
  {
    return hostBuilder
    .ConfigureServices(
      (context, services) =>
      {
        var settings = services.BuildServiceProvider().GetRequiredService<ISettings>();
        var logger = services.BuildServiceProvider().GetRequiredService<ILogger>();

        try
        {
          var onspringClient = new OnspringClient(
            settings.Onspring.BaseUrl,
            settings.Onspring.ApiKey
          );

          services.AddSingleton<IOnspringClient>(onspringClient);
        }
        catch (Exception ex)
        {
          logger.Fatal(
            ex,
            "Unable to create Onspring client: {Message}",
            ex.Message
          );

          throw;
        }
      }
    );
  }

  public static IHostBuilder AddSerilog(this IHostBuilder hostBuilder)
  {
    return hostBuilder
    .UseSerilog(
      (context, services, config) =>
      {
        var options = services.GetRequiredService<IOptions<AppOptions>>().Value;
        var azureGroupDestructPolicy = services.GetRequiredService<IAzureGroupDestructuringPolicy>();

        config
        .Destructure.With(new IDestructuringPolicy[]
          {
            azureGroupDestructPolicy,
          }
        )
        .MinimumLevel.Debug()
        .Enrich.FromLogContext()
        .WriteTo.File(
          new CompactJsonFormatter(),
          Path.Combine(
            AppContext.BaseDirectory,
            $"{DateTime.Now:yyyy_MM_dd_HHmmss}_output",
            "log.json"
          )
        )
        .WriteTo.Console(
          restrictedToMinimumLevel: options.LogLevel,
          theme: AnsiConsoleTheme.Code
        );
      }
    );
  }
}