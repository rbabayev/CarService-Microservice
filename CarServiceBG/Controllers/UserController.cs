using CarService.Entities.Entities;
using CarServiceBG.DTOs;
using CarServiceBG.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarServiceBG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IPhotoService _photoService;

        public UserController(UserManager<User> userManager, IPhotoService photoService)
        {
            _userManager = userManager;
            _photoService = photoService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return NotFound();

            var response = new UserResponseDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                ProfileImageUrl = user.ProfileImageUrl,
                ShopImageUrl = user.ShopImageUrl,
                Role = user.Role
            };

            return Ok(response);
        }

        [HttpPut("update-photo")] // Frontend'den gelen PUT isteğini karşılar
        [Authorize] // Eğer sadece giriş yapmış kullanıcıların fotoğrafını güncellemesine izin veriyorsanız
        public async Task<IActionResult> UpdatePhoto([FromForm] UserPhotoUpdateDto dto) // DTO'yu tanımlamanız gerekecek
        {
            // Token'dan kullanıcı ID'sini alın (frontend'den gönderilen UserId'ye güvenmek yerine daha güvenli)
            var userIdFromToken = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userIdFromToken == null || !Guid.TryParse(userIdFromToken, out Guid parsedUserIdFromToken))
            {
                return Unauthorized("User ID not found in token or invalid format.");
            }

            var user = await _userManager.FindByIdAsync(parsedUserIdFromToken.ToString());
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            if (dto.Photo == null || dto.Photo.Length == 0)
            {
                return BadRequest(new { message = "No photo file provided." });
            }

            // Fotoğrafı yüklemek için IPhotoService'i kullanın
            var photoUrl = await _photoService.UploadImageAsync(new PhotoCreateDto { File = dto.Photo });

            if (string.IsNullOrEmpty(photoUrl))
            {
                return BadRequest(new { message = "Failed to upload photo." });
            }

            user.ProfileImageUrl = photoUrl; // Kullanıcının profil fotoğraf URL'sini güncelleyin
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Failed to update user's profile image.", errors = result.Errors.Select(e => e.Description) });
            }

            return Ok(new { message = "Profile photo updated successfully.", photoUrl });
        }
    }
}
