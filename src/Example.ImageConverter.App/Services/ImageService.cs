namespace Example.ImageConverter.App.Services;

public class ImageService
{
    public async Task<Stream> ConvertToWebpAsync(Stream source, CancellationToken cancellationToken = default)
    {
        var imageSource = await Image.LoadAsync(source, cancellationToken);

        MemoryStream destination = new();
        await imageSource.SaveAsWebpAsync(destination, cancellationToken);

        return destination;
    }
}
