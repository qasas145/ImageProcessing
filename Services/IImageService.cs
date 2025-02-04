using ImageProcessing.Models;
using SixLabors.ImageSharp;

namespace ImageProcessing.Services
{
    public interface IImageService
    {
        Task<byte[]> SaveImageAsync(Image image, string name, int resizeWidth);
        Task Process(IEnumerable<ImageInputModel> images);
        Task<IEnumerable<string>> GetAllImages();
        Task<Stream> GetThumbnail(string id);
        Task<Stream> GetFullScreen(string id);
    }
}
