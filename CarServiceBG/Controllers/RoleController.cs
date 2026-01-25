using CarService.DataAccess.Database;
using CarService.Entities.Entities;
using CarServiceBG.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarServiceBG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly CarServiceDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ILogger<RoleController> _logger;

        public RoleController(CarServiceDbContext context, UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager, ILogger<RoleController> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }


        [HttpGet]
        public IActionResult GetAllRoles()
        {
            var roles = _roleManager.Roles.ToList();
            return Ok(roles);
        }

        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] RoleCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest(new { error = "Role name is required" });

            var roleExist = await _roleManager.RoleExistsAsync(dto.Name);
            if (!roleExist)
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole<Guid>(dto.Name));

                if (roleResult.Succeeded)
                {
                    _logger.LogInformation($"The Role {dto.Name} has been created successfully");
                    return Ok(new { result = $"The role {dto.Name} has been added successfully" });
                }
                else
                {
                    return BadRequest(new { error = "Failed to create role" });
                }
            }

            return BadRequest(new { error = "Role already exists" });
        }


        [HttpGet]
        [Route("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return Ok(users);
        }


        [HttpGet]
        [Route("GetUserRoles")]
        public async Task<IActionResult> GetUserRoles(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogInformation($"The user with the {email} does not exist");
                return BadRequest(new
                {
                    error = "User doest not exist"
                });
            }

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(roles);
        }


        [HttpGet]
        [Route("GetUserRoleById")]
        public async Task<IActionResult> GetUserRoleById(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                _logger.LogInformation($"User with ID {userId} does not exist");
                return BadRequest(new { error = "User does not exist" });
            }

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(roles);
        }



        [HttpPost]
        [Route("RemoveUserFromRole")]
        public async Task<IActionResult> RemoveUserFromRole([FromQuery] string email, [FromQuery] string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogInformation($"The user with the {email} does not exist");
                return BadRequest(new
                {
                    error = "User doest not exist"
                });
            }
            var roleExist = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                _logger.LogInformation($"The role {roleName} does not exist");
                return BadRequest(new
                {
                    error = "Role does not exist"
                });
            }

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            if (result.Succeeded)
            {
                return Ok(new
                {
                    result = $"User has been removed from role {roleName}"
                });
            }
            return BadRequest(new
            {
                error = $"Unable to remove User {email} from role {roleName}"
            });
        }

        [HttpGet("DebugRoles")]
        public IActionResult DebugRoles()
        {
            var roles = _roleManager.Roles.Select(r => r.Name).ToList();
            return Ok(roles);
        }


    }


}


