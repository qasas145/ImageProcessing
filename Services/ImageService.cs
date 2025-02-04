using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using ImageProcessing.Models;
using ImageProcessing.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Drawing;

namespace ImageProcessing.Services
{
    public class ImageService : IImageService
    {
        #region Fields
        private int fullWidth = 800;
        private int ThumbnailWudth = 300;
        private IServiceScopeFactory _serviceScopeFactory;
        private readonly ApplicationDbContext _dbContext;

        #endregion

        #region Methods
        public ImageService(IServiceScopeFactory serviceScopeFactory, ApplicationDbContext dbContext)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _dbContext = dbContext;
        }

        public async Task Process(IEnumerable<ImageInputModel> images)
        {

            var tasks = images.Select(image => Task.Run(async () =>
            {
                try
                {

                    var dbContext = _serviceScopeFactory
                        .CreateScope()
                        .ServiceProvider
                        .GetService<ApplicationDbContext>();

                    using var imageResult = await Image.LoadAsync(image.Content);
                    var original = await SaveImageAsync(imageResult, $"Original_{image?.FileName}", imageResult.Width);
                    var fullScreen = await SaveImageAsync(imageResult, $"FullScreen_{image?.FileName}", fullWidth);
                    var thumbnail = await SaveImageAsync(imageResult, $"Thumbnail_{image?.FileName}", ThumbnailWudth);
                    var imageData = new ImageData
                    {
                        OriginalFileName = image.FileName,
                        OriginalType = image.Type,
                        OriginalContent = original,
                        ThumbnailContent = thumbnail,
                        FullScreenContent = fullScreen,
                    };

                    await dbContext.ImageData.AddAsync(imageData);
                    await dbContext.SaveChangesAsync();

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
        public async Task<byte[]> SaveImageAsync(Image image, string name , int resizeWidth)
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

            var memoryStream = new MemoryStream();
            await image.SaveAsJpegAsync(memoryStream, new JpegEncoder
            {
                Quality = 75
            });
            return memoryStream.ToArray();
        }


        public async Task<IEnumerable<string>> GetAllImages()
        {
            
            return await _dbContext.ImageData.Select(i => i.Id.ToString()).ToListAsync();
        }
        public Task<Stream> GetThumbnail(string id)
        {
            return this.GetImageContentAsStream(id, "Thumbnail");
        }
        public Task<Stream> GetFullScreen(string id)
        {
            return this.GetImageContentAsStream(id, "FullScreen");
        }

        public async Task<Stream> GetImageContentAsStream(string id, string size)
        {

            var data = _dbContext.Database;
            var dbConnection = (SqlConnection)data.GetDbConnection();
            var command = new SqlCommand(
                $"select {size}Content from ImageData where Id = @id;",dbConnection
            );
            command.Parameters.Add(new SqlParameter("@id", id));
            dbConnection.Open();
            var reader = await command.ExecuteReaderAsync();
            Stream result = null;
            if (reader.HasRows)
            {
                while (reader.Read()) result = reader.GetStream(0);
            }
            reader.Close();
            return result;
        }
        #endregion
    }
}
