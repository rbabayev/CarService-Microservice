
using CarServiceBG.DTOs;

namespace CarServiceBG.Services
{
    public interface IPhotoService
    {
        Task<string> UploadImageAsync(PhotoCreateDto dto);
    }
}
