using CarService.DataAccess.Database;
using CarService.Entities.Entities;
using CarServiceBG.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceBG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRolesController : ControllerBase
    {
        private readonly CarServiceDbContext _context;
        private readonly UserManager<User> _userManager;

        public UserRolesController(CarServiceDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Guid userId, string roleName, Guid roleId, CancellationToken token)
        {
            User? user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return BadRequest(new { Message = "User can't find" });
            }
            IdentityResult result = await _userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(s => s.Description));
            }
            return NoContent();
        }

        [HttpPost("Assign")]
        public async Task<IActionResult> AssignRole([FromBody] AddUserToRoleDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId.ToString());
            if (user == null)
                return BadRequest(new { message = "User not found" });

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains(dto.RoleName))
                return BadRequest(new { message = $"User already has role '{dto.RoleName}'" });

            var result = await _userManager.AddToRoleAsync(user, dto.RoleName);
            user.Role = dto.RoleName;
            await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            return Ok(new { message = $"User added to role '{dto.RoleName}' successfully" });
        }


        [HttpPost("Remove")]
        public async Task<IActionResult> RemoveRole([FromBody] AddUserToRoleDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId.ToString());
            if (user == null)
                return BadRequest(new { message = "User not found" });

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains(dto.RoleName))
                return BadRequest(new { message = $"User doesn't have role '{dto.RoleName}'" });

            var result = await _userManager.RemoveFromRoleAsync(user, dto.RoleName);
            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));
            user.Role = null;
            await _userManager.UpdateAsync(user);

            return Ok(new { message = $"Role '{dto.RoleName}' removed from user" });
        }



        [HttpDelete]
        public async Task<IActionResult> Delete(Guid userId, string roleName, Guid roleId, CancellationToken token)
        {
            User? user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return BadRequest(new { Message = "User can't find" });
            }
            IdentityResult result = await _userManager.RemoveFromRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(s => s.Description));
            }
            return NoContent();
        }
    }
}
