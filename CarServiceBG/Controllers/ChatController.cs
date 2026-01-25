using CarService.Business.Abstract;
using CarServiceBG.DTOs;
using CarServiceBG.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CarServiceBG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IUserService _userService;
        private readonly IChatService _chatService;

        public ChatController(IHubContext<ChatHub> hubContext, IUserService userService, IChatService chatService)
        {
            _hubContext = hubContext;
            _userService = userService;
            _chatService = chatService;
        }

        // Kullanıcı adını alarak chat odasına bağlanması için yardımcı ol
        [Authorize(Roles = "User,Worker,Admin")]
        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
        {
            var user = await _userService.GetUserByUsernameAsync(dto.Username);
            if (user == null)
            {
                return BadRequest(new { Message = "User not found" });
            }

            // Kullanıcı bulunduysa mesajı ilet
            await _hubContext.Clients.Group(dto.Username)
                .SendAsync("ReceiveMessage", dto.Username, dto.Message);

            return Ok(new { Message = "Message sent" });
        }

        [Authorize(Roles = "User,Worker,Admin")]
        [HttpGet("GetMessages/{userId1}/{userId2}")]
        public async Task<IActionResult> GetMessages(Guid userId1, Guid userId2)
        {
            var allMessages = await _chatService.GetMessagesAsync(userId1, userId2);

            var result = allMessages.Select(m => new MessageDto
            {
                MessageTxt = m.MessageTxt,
                DateTime = m.DateTime,
                UserId = m.UserId,
                RecipientUserId = m.RecipientUserId,
                SenderName = m.User?.FullName ?? "Unknown"
            }).ToList();

            return Ok(result);
        }



    }
}
