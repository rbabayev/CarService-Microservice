using CarService.Business.Abstract;
using CarService.DataAccess.Database;
using CarService.Entities.Entities;
using CarServiceBG.DTOs;
using CarServiceBG.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CarServiceBG.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IShopService _shopService;
        private readonly IWorkerService _workerService;
        private readonly IConfiguration _configuration;
        private readonly CarServiceDbContext _context;
        private readonly IUserService _userService;
        private readonly IPhotoService _photoService;

        public AuthController(
            UserManager<User> userManager,
          IUserService userService,
          IConfiguration configuration,
           SignInManager<User> signInManager,
          IPhotoService photoService,
          CarServiceDbContext context,
          IWorkerService workerService,
          IShopService shopService,
          RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _userService = userService;
            _signInManager = signInManager;
            _configuration = configuration;
            _context = context;
            _photoService = photoService;
            _shopService = shopService;
            _roleManager = roleManager;
            _userManager = userManager;
            _workerService = workerService;
        }


        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return BadRequest(new { error = "Email already in use" });
            }


            string? profileImageUrl = null;
            string? shopImageUrl = null;

            if (registerDto.Role == "Worker" && registerDto.Photo != null)
            {

                var photoDto = new PhotoCreateDto { File = registerDto.Photo };
                profileImageUrl = await _photoService.UploadImageAsync(photoDto);
            }
            else if (registerDto.Role == "Seller" && registerDto.Photo != null)
            {

                var photoDto = new PhotoCreateDto { File = registerDto.Photo };
                shopImageUrl = await _photoService.UploadImageAsync(photoDto);
            }


            var user = new User
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
                ProfileImageUrl = profileImageUrl,
                ShopImageUrl = shopImageUrl,
                ShopName = registerDto.ShopName,
                ShopCategory = registerDto.ShopCategory,
                ExperienceYears = registerDto.ExperienceYears,
                WorkerCategory = registerDto.WorkerCategory,
                Role = registerDto.Role

            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new { Errors = result.Errors.Select(x => x.Description).ToList() });
            }


            var roleName = registerDto.Role;
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                role = new IdentityRole<Guid>(roleName);
                var roleResult = await _roleManager.CreateAsync(role);
                if (!roleResult.Succeeded)
                {
                    return BadRequest(new { error = "Failed to create role" });
                }
            }

            await _userManager.AddToRoleAsync(user, roleName);


            if (registerDto.Role == "Seller")
            {
                var shop = new Shop
                {
                    Id = Guid.NewGuid(),
                    ShopName = registerDto.ShopName,
                    ShopCategory = registerDto.ShopCategory,
                    ShopImageUrl = shopImageUrl,
                    UserId = user.Id,
                    Point = 0
                };
                await _shopService.CreateShopAsync(shop);
            }
            else if (registerDto.Role == "Worker")
            {
                var worker = new Worker
                {
                    Id = Guid.NewGuid(),
                    FullName = registerDto.WorkerName,
                    WorkerCategory = registerDto.WorkerCategory,
                    ProfileImageUrl = profileImageUrl,
                    WorkerPoint = 0,
                    UserId = user.Id
                };
                Console.WriteLine("Created UserId: " + user.Id);
                await _workerService.CreateWorkerAsync(worker);
            }

            return Ok(new { Message = "User created successfully!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {

            var user = await _userManager.FindByNameAsync(loginDto.Username);
            if (user == null)
            {
                return Unauthorized(new { Message = "Invalid credentials" });
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, false, false);
            if (result.Succeeded)
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {

                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                     new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                      new Claim(ClaimTypes.Name, user.UserName ?? "Unknown"),
                        new Claim(ClaimTypes.Email, user.Email ?? ""),

                };

                authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

                var token = new JwtSecurityToken(
                    audience: _configuration["JWT:Audience"],
                    issuer: _configuration["JWT:Issuer"],
                    expires: DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JWT:ExpiryMinutes"])),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!)),
                    SecurityAlgorithms.HmacSha256
                    )
                    );


                Guid? shopId = null;
                if (userRoles.Contains("Seller"))
                {
                    var shop = await _shopService.GetShopByUserIdAsync(user.Id);
                    if (shop != null)
                    {
                        shopId = shop.Id;
                    }
                }

                return Ok(new
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    UserId = user.Id,
                    ShopId = shopId,
                    Roles = userRoles

                });

            }

            return Unauthorized(new { Message = "Invalid credentials" });

        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId.ToString());
            if (user == null)
                return NotFound(new { message = "User not found." });

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (!result.Succeeded)
                return BadRequest(new { message = "Password update failed.", errors = result.Errors.Select(e => e.Description) });

            return Ok(new { message = "Password updated successfully." });
        }



        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(string email, CancellationToken cancellationToken)
        {
            User? user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest(new { Message = "User can't find" });
            }

            string token = await _userManager.GeneratePasswordResetTokenAsync(user);

            return Ok(new { Token = token });
        }

        [HttpPost("change-password-token")]
        public async Task<IActionResult> ChangePasswordUsingToken(ChangePasswordUsingTokenDto dto, CancellationToken cancellationToken)
        {
            User? user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return BadRequest(new { Message = "User can't find" });
            }
            IdentityResult result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(s => s.Description));
            }
            return NoContent();

        }

        [HttpDelete("DeleteByEmail")]
        public async Task<IActionResult> DeleteByEmail([FromQuery] string email)
        {
            var decodedEmail = Uri.UnescapeDataString(email);
            var user = await _userManager.FindByEmailAsync(decodedEmail);
            if (user == null)
                return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "User deleted" });
        }


    }
}
