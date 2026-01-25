using CarServiceBG.DTOs;
using CarServiceBG.Settings;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace CarServiceBG.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;

        public PhotoService(IOptions<CloudinarySettings> options)
        {
            var settings = options.Value;

            if (string.IsNullOrEmpty(settings.CloudName))
                throw new Exception("Cloudinary CloudName is missing");

            var account = new Account(
                settings.CloudName,
                settings.ApiKey,
                settings.ApiSecret
            );

            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadImageAsync(PhotoCreateDto dto)
        {
            ImageUploadResult uploadedResult = new ImageUploadResult();

            IFormFile? file = dto.File;
            if (file?.Length > 0)
            {
                using (Stream stream = file.OpenReadStream())
                {
                    ImageUploadParams uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.Name, stream)
                    };

                    uploadedResult = await _cloudinary.UploadAsync(uploadParams);

                    if (uploadedResult.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return uploadedResult.Url.ToString();
                    }
                }
            }
            return string.Empty;
        }
    }
}
