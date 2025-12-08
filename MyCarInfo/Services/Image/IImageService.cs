using Microsoft.AspNetCore.Components.Forms;
using MyCarInfo.Models;

namespace MyCarInfo.Services.Image
{
    public interface IImageService
    {
        Task<ImageUploadResult> SaveImagesAsync(
            IEnumerable<IBrowserFile> files,
            string relativeFolder,
            long maxFileSize,
            int maxFileCount,
            IReadOnlyCollection<string> allowedExtensions);

        void DeleteImage(string? relativePath);
    }
}
