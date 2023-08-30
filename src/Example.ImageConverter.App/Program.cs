using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Example.ImageConverter.App.Services;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine($"Start {typeof(Program).Assembly.FullName}");

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(builder =>
    {
        var stage = Environment.GetEnvironmentVariable("NETCORE_STAGE");

        builder.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{stage}.json", optional: true);
    })
    .ConfigureServices(services =>
    {
        // Add your features
        services.AddOptions<AppConfiguration>()
            .Configure<IConfiguration>((option, configuration) =>
            {
                configuration.GetSection(AppConfiguration.Name).Bind(option);
            });

        services.AddScoped<ImageService>();
        services.AddHostedService<TestHostedService>();
    })
    .Build();

await host.StartAsync(CancellationToken.None);