using Microsoft.AspNetCore.Components.Forms;
using MyCarInfo.Models;

namespace MyCarInfo.Services.Image
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _environment;

        public ImageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<ImageUploadResult> SaveImagesAsync(
            IEnumerable<IBrowserFile> files,
            string relativeFolder,
            long maxFileSize,
            int maxFileCount,
            IReadOnlyCollection<string> allowedExtensions)
        {
            var result = new ImageUploadResult();
            var normalizedFolder = relativeFolder.Trim('/');
            var uploadsRoot = Path.Combine(_environment.WebRootPath, normalizedFolder.Replace('/', Path.DirectorySeparatorChar));

            Directory.CreateDirectory(uploadsRoot);

            foreach (var file in files.Take(maxFileCount))
            {
                var extension = Path.GetExtension(file.Name).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                {
                    result.Errors.Add("Моля, качи изображение във формат JPG, PNG или WEBP.");
                    continue;
                }

                if (file.Size > maxFileSize)
                {
                    result.Errors.Add("Всяка снимка трябва да бъде по-малка от 5 MB.");
                    continue;
                }

                var fileName = $"{Guid.NewGuid()}{extension}";
                var savePath = Path.Combine(uploadsRoot, fileName);

                await using var stream = File.Create(savePath);
                await file.OpenReadStream(maxFileSize).CopyToAsync(stream);

                var relativePath = $"/{normalizedFolder}/{fileName}";
                if (!result.SavedPaths.Contains(relativePath))
                {
                    result.SavedPaths.Add(relativePath);
                }
            }

            return result;
        }

        public void DeleteImage(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return;
            }

            var trimmedPath = relativePath.TrimStart('/');
            var absolutePath = Path.Combine(_environment.WebRootPath, trimmedPath.Replace('/', Path.DirectorySeparatorChar));

            if (File.Exists(absolutePath))
            {
                File.Delete(absolutePath);
            }
        }
    }
}
