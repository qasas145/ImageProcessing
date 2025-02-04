namespace ImageProcessing.Models
{
    public class ImageData
    {
        public ImageData() => Id = Guid.NewGuid();
        public Guid Id { get; set; }
        public string OriginalFileName { get;set; }
        public string OriginalType { get; set; }
        public byte[] OriginalContent { get;set; }
        public byte[] ThumbnailContent { get; set; }
        public byte[] FullScreenContent { get;set; }

    }
}
