namespace MyCarInfo.Models
{
    public class ImageUploadResult
    {
        public List<string> SavedPaths { get; } = new();

        public List<string> Errors { get; } = new();

        public bool HasErrors => Errors.Count > 0;
    }
}
