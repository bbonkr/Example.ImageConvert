using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Example.ImageConverter.App.Services;

public class TestHostedService : IHostedService
{
    private readonly AppConfiguration _appConfiguration;
    private readonly ImageService _imageService;

    public TestHostedService(
        IOptionsMonitor<AppConfiguration> appConfigurationAccessor,
        ImageService imageService)
    {
        _appConfiguration = appConfigurationAccessor.CurrentValue;
        _imageService = imageService;
    }

    private async Task ConvertAsync(string path, string filter, CancellationToken cancellationToken = default)
    {
        var files = Directory.GetFiles(path, filter);

        ParallelOptions parallelOptions = new()
        {
            CancellationToken = cancellationToken,
        };

        await Parallel.ForEachAsync(files, parallelOptions, async (file, ct) =>
        {
            FileInfo fileInfo = new(file);

            if (fileInfo.Extension != ".webp")
            {
                using var stream = File.OpenRead(file);

                var webpStream = await _imageService.ConvertToWebpAsync(stream, cancellationToken);

                if (webpStream != null)
                {
                    var fileNameTokens = fileInfo.Name.Split('.');
                    if (fileNameTokens.Length > 1)
                    {
                        fileNameTokens = fileNameTokens.Take(fileNameTokens.Length - 1).ToArray();
                    }
                    var filename = string.Join(".", fileNameTokens);

                    var webpFilename = $"{filename}.webp";
                    var webpFileOutput = Path.Combine(path, "output");
                    if (!Directory.Exists(webpFileOutput))
                    {
                        Directory.CreateDirectory(webpFileOutput);
                    }

                    var webpFilePath = Path.Combine(webpFileOutput, webpFilename);

                    if (File.Exists(webpFilePath))
                    {
                        int num = 1;
                        do
                        {
                            webpFilename = $"{filename}-{num}.webp";
                            webpFilePath = Path.Combine(path, "output", webpFilename);
                            num += 1;
                        } while (File.Exists(webpFilePath));
                    }


                    if (!File.Exists(webpFilePath))
                    {
                        using var webpFileStream = new FileStream(webpFilePath, FileMode.CreateNew, FileAccess.Write);
                        webpStream.Seek(0, SeekOrigin.Begin);
                        await webpStream.CopyToAsync(webpFileStream, cancellationToken);
                        webpStream.Close();
                        webpFileStream.Close();
                    }
                }
            }
        });
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await ConvertAsync(_appConfiguration.Source, _appConfiguration.Filter, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
