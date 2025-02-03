namespace ImageProcessing.Models
{
    public class ImageInputModel
    {
        public string? FileName { get; set; }
        public string Type { get; set; } = null!;
        public Stream Content { get; set; } = null!;

    }
}
