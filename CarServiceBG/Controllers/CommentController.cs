using CarServiceBG.DTOs;
using CarServiceBG.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarServiceBG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _service;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CommentController(ICommentService service, IHttpContextAccessor httpContextAccessor)
        {
            _service = service;
            _httpContextAccessor = httpContextAccessor;
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost("AddComment")]
        public async Task<IActionResult> AddComment([FromBody] CommentDto dto)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            var userNameClaim = _httpContextAccessor.HttpContext?.User?.Identity?.Name;

            if (userIdClaim == null)
                return Unauthorized("User not authenticated.");

            dto.UserId = Guid.Parse(userIdClaim.Value);
            dto.UserName = userNameClaim ?? "Anonymous";
            dto.DateTime = DateTime.UtcNow;

            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        [HttpGet("workers/{workerId}")]
        public async Task<IActionResult> GetByWorker(Guid workerId)
        {
            var comments = await _service.GetByWorkerAsync(workerId);
            return Ok(comments);
        }

    }
}
