using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using ImageProcessing.Models;

namespace ImageProcessing.Services
{
    public class ImageService : IImageService
    {
        private int fullWidth = 800;
        private int ThumbnailWudth = 300;
        public async Task Process(IEnumerable<ImageInputModel> images)
        {
            var tasks = images.Select(image => Task.Run(async () =>
            {
                try
                {
                    using var imageResult = await Image.LoadAsync(image.Content);
                    await SaveImageAsync(imageResult, $"Original_{image?.FileName}", imageResult.Width);
                    await SaveImageAsync(imageResult, $"FullScreen_{image?.FileName}", fullWidth);
                    await SaveImageAsync(imageResult, $"Thumbnail_{image?.FileName}", ThumbnailWudth);
                }
                catch(Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.ForegroundColor = ConsoleColor.White;
                }

            }));
            await Task.WhenAll(tasks);
        }
        public async Task SaveImageAsync(Image image, string name , int resizeWidth)
        {
            var width = image.Width;
            var height = image.Height;
            if (resizeWidth < width)
            {
                height = (int)((double)resizeWidth / width * height);
                width = resizeWidth;
            }
            image.Mutate(i => i.Resize(width, height));
            image.Metadata.ExifProfile = null;

            await image.SaveAsJpegAsync(name, new JpegEncoder
            {
                Quality = 75
            });
        }
    }
}
