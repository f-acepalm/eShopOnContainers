using Autofac.Extensions.DependencyInjection;
using Loyalty.API;
using Loyalty.API.DataAccess;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;

IConfiguration configuration = GetConfiguration();

Log.Logger = CreateSerilogLogger(configuration);

try
{
    Log.Information("Configuring web host ({ApplicationContext})...", Program.AppName);
    IWebHost host = CreateHostBuilder(configuration, args);

    Log.Information("Applying migrations ({ApplicationContext})...", Program.AppName);
    host.MigrateDbContext<LoyaltyContext>((context, services) =>
    {
        IWebHostEnvironment env = services.GetService<IWebHostEnvironment>();
        ILogger<LoyaltyContextSeed> logger = services.GetService<ILogger<LoyaltyContextSeed>>();

        new LoyaltyContextSeed().SeedAsync(context, env, logger).Wait();
    });

    Log.Information("Starting web host ({ApplicationContext})...", Program.AppName);
    host.Run();

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", Program.AppName);
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

IWebHost CreateHostBuilder(IConfiguration configuration, string[] args)
{
#pragma warning disable CS0618 // Type or member is obsolete
    return WebHost.CreateDefaultBuilder(args)
      .ConfigureAppConfiguration(x => x.AddConfiguration(configuration))
      .CaptureStartupErrors(false)
      //.ConfigureKestrel(options =>
      //{
      //    var ports = GetDefinedPorts(configuration);
      //    options.Listen(IPAddress.Any, ports.httpPort, listenOptions =>
      //    {
      //        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
      //    });
      //    options.Listen(IPAddress.Any, ports.grpcPort, listenOptions =>
      //    {
      //        listenOptions.Protocols = HttpProtocols.Http2;
      //    });

      //})
      .UseStartup<Startup>()
      .UseContentRoot(Directory.GetCurrentDirectory())
      //.UseWebRoot("Pics")
      .UseSerilog()
      .Build();
#pragma warning restore CS0618 // Type or member is obsolete
}

Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
{
    var seqServerUrl = configuration["Serilog:SeqServerUrl"];
    var logstashUrl = configuration["Serilog:LogstashgUrl"];
    return new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .Enrich.WithProperty("ApplicationContext", Program.AppName)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.Seq(string.IsNullOrWhiteSpace(seqServerUrl) ? "http://seq" : seqServerUrl)
        .WriteTo.Http(string.IsNullOrWhiteSpace(logstashUrl) ? "http://logstash:8080" : logstashUrl)
        .ReadFrom.Configuration(configuration)
        .CreateLogger();
}

IConfiguration GetConfiguration()
{
    IConfigurationBuilder builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();

    IConfigurationRoot config = builder.Build();

    //if (config.GetValue<bool>("UseVault", false))
    //{
    //    TokenCredential credential = new ClientSecretCredential(
    //        config["Vault:TenantId"],
    //        config["Vault:ClientId"],
    //        config["Vault:ClientSecret"]);
    //    //builder.AddAzureKeyVault(new Uri($"https://{config["Vault:Name"]}.vault.azure.net/"), credential);        
    //}

    return builder.Build();
}

public partial class Program
{
    public static string Namespace = typeof(Startup).Namespace;
    public static string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);
}