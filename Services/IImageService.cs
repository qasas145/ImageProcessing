using ImageProcessing.Models;
using SixLabors.ImageSharp;

namespace ImageProcessing.Services
{
    public interface IImageService
    {
        Task SaveImageAsync(Image image, string name, int resizeWidth);
        Task Process(IEnumerable<ImageInputModel> images);
    }
}
