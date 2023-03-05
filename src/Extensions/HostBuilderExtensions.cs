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

  public static IHostBuilder AddConfiguration(this IHostBuilder hostBuilder, Options options)
  {
    return hostBuilder
    .ConfigureAppConfiguration(
      (context, config) =>
        config
        .AddJsonFile(
          options.ConfigFile!,
          optional: false,
          reloadOnChange: true
        )
        .AddEnvironmentVariables()
    );
  }

  public static IHostBuilder AddSerilog(this IHostBuilder hostBuilder, Options options)
  {
    return hostBuilder
    .UseSerilog(
      (context, services, config) =>
        config
        .Destructure.With(new IDestructuringPolicy[]
          {
            services.GetRequiredService<IAzureGroupDestructuringPolicy>(),
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
        )
    );
  }

  public static IHostBuilder AddServices(this IHostBuilder hostBuilder)
  {
    return hostBuilder
    .ConfigureServices(
      (context, services) =>
      {
        services.AddSingleton<ISettings, Settings>();
        services.AddSingleton<IAzureGroupDestructuringPolicy, AzureGroupDestructuringPolicy>();
        services.AddSingleton<IOnspringService, OnspringService>();
        services.AddSingleton<IGraphService, GraphService>();
        services.AddSingleton<IProcessor, Processor>();
        services.AddSingleton<ISyncer, Syncer>();
      }
    );
  }
}