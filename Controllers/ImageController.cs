using ImageProcessing.Models;
using ImageProcessing.Services;
using Microsoft.AspNetCore.Mvc;

namespace ImageProcessing.Controllers
{
    public class ImageController : Controller
    {
        private readonly IImageService _imageService;
        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }
        public IActionResult Upload()
        {
            return View();
        }
        [HttpPost]
        [RequestSizeLimit(100* 1024 * 1024)] // to prevent the user from uploading files which size is more than 100 MB
        public async Task<IActionResult> Upload(IFormFile[] images)
        {
            var inputImages = images.Select(i => new ImageInputModel
            {
                FileName = i.FileName,
                Type = i.ContentType,
                Content = i.OpenReadStream(),

            });
            await _imageService.Process(inputImages);
            return RedirectToAction(nameof(Done));
        }
        public IActionResult Done()
        {
            return View();
        }
    }
}
