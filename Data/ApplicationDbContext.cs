using ImageProcessing.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageProcessing.Data
{
    public class ApplicationDbContext  : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<ImageData> ImageData { get; set; }
    }
}
