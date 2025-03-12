namespace OnspringAzureADSyncer.Extensions;

public static class HostBuilderExtensions
{
  public static IHostBuilder AddGraphClient(this IHostBuilder hostBuilder)
  {
    return hostBuilder
    .ConfigureServices(static (context, services) =>
    {
      var settings = services.BuildServiceProvider().GetRequiredService<ISettings>();

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
        ["https://graph.microsoft.com/.default"]
      );

      services.AddSingleton(graphServiceClient);
    });
  }

  public static IHostBuilder AddOnspringClient(this IHostBuilder hostBuilder)
  {
    return hostBuilder
      .ConfigureServices(static (context, services) =>
      {
        var settings = services.BuildServiceProvider().GetRequiredService<ISettings>();

        var onspringClient = new OnspringClient(
          settings.Onspring.BaseUrl,
          settings.Onspring.ApiKey
        );

        services.AddSingleton<IOnspringClient>(onspringClient);
      });
  }

  public static IHostBuilder AddSerilog(this IHostBuilder hostBuilder)
  {
    return hostBuilder
    .UseSerilog(
      static (context, services, config) =>
      {
        var options = services.GetRequiredService<IOptions<AppOptions>>().Value;
        var azureGroupDestructPolicy = services.GetRequiredService<IAzureGroupDestructuringPolicy>();
        var azureUserDestructPolicy = services.GetRequiredService<IAzureUserDestructuringPolicy>();

        var logPath = Path.Combine(
          AppContext.BaseDirectory,
          $"{DateTime.Now:yyyy_MM_dd_HHmmss}_output",
          "log.txt"
        );

        config
          .Destructure.With([
            azureGroupDestructPolicy,
            azureUserDestructPolicy
          ])
          .MinimumLevel.Debug()
          .Enrich.FromLogContext()
          .WriteTo.File(new CompactJsonFormatter(), logPath, options.LogLevel);
      }
    );
  }
}