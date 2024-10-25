namespace GamingApp.Web.Services;

public interface IImageOptimizationService
{
    string GetOptimizedImageUrl(string originalUrl, int width, int height);
}

public class ImageOptimizationService : IImageOptimizationService
{
    private readonly IConfiguration _configuration;
    private readonly string _imageServiceUrl;

    public ImageOptimizationService(IConfiguration configuration)
    {
        _configuration = configuration;
        _imageServiceUrl = _configuration["ImageService:BaseUrl"] ?? "";
    }

    public string GetOptimizedImageUrl(string originalUrl, int width, int height)
    {
        if (string.IsNullOrEmpty(originalUrl)) return originalUrl;
        return $"{_imageServiceUrl}/resize?url={Uri.EscapeDataString(originalUrl)}&w={width}&h={height}&quality=80";
    }
}
