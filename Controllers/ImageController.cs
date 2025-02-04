using ImageProcessing.Models;
using ImageProcessing.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

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

        public async Task<IActionResult> All()
        {
            return View(await _imageService.GetAllImages());
        }
        public async Task<IActionResult> Thumbnail(string id)
        {
            return this.File(await _imageService.GetThumbnail(id), "image/jpeg");
            //return this.ReturnImage(await _imageService.GetThumbnail(id));
        }
        public async Task<IActionResult> FullScreen(string id)
        {
            return ReturnImage(await _imageService.GetFullScreen(id));

        }
        public IActionResult ReturnImage(Stream image)
        {
            var headers = this.Response.GetTypedHeaders();
            headers.CacheControl = new CacheControlHeaderValue
            {
                Public = true,
                MaxAge = TimeSpan.FromMinutes(10)
            };
            headers.Expires = new DateTimeOffset(DateTime.UtcNow.AddMinutes(10));
            return this.File(image, "image/jpeg");
        }
    }
}
