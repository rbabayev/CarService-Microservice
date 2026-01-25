using CarService.Business.Abstract;
using CarService.Entities.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace CarServiceBG.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private static readonly ConcurrentDictionary<Guid, string> OnlineUsers = new();

        private readonly IWorkerService _workerService;
        private readonly IChatService _chatService;

        public ChatHub(IWorkerService workerService, IChatService chatService)
        {
            _workerService = workerService;
            _chatService = chatService;
        }

        public override Task OnConnectedAsync()
        {
            var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out Guid userId))
            {
                OnlineUsers[userId] = Context.ConnectionId; // ✅ Add or update user connection
                Console.WriteLine($"✅ Connected: {userId} -> {Context.ConnectionId}");
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out Guid userId))
            {
                OnlineUsers.TryRemove(userId, out _); // ✅ Remove disconnected user
                Console.WriteLine($"❌ Disconnected: {userId}");
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(Guid receiverId, string message, string senderName)
        {
            var senderIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(senderIdClaim, out Guid senderId))
                return;

            // 1. Check if chat exists
            var existingChats = await _chatService.GetAllChatsAsync();
            var chat = existingChats.FirstOrDefault(c =>
                (c.SenderId == senderId && c.ReceiverId == receiverId) ||
                (c.SenderId == receiverId && c.ReceiverId == senderId));

            if (chat == null)
            {
                chat = new Chat
                {
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    Messages = new List<Message>()
                };
                await _chatService.AddChatAsync(chat);
            }

            // 2. Message'i chat ile kaydet
            var msg = new Message
            {
                MessageTxt = message,
                DateTime = DateTime.UtcNow,
                UserId = senderId,
                RecipientUserId = receiverId,
                ChatId = chat.Id // ✅ ChatId'yi set ediyoruz
            };

            await _chatService.SaveMessageAsync(msg);

            // 3. Send to Receiver
            if (OnlineUsers.TryGetValue(receiverId, out string receiverConnId))
            {
                await Clients.Client(receiverConnId).SendAsync("ReceiveMessage", senderName, message, senderId.ToString());
            }

            // 4. Send to Sender
            await Clients.Caller.SendAsync("ReceiveMessage", "You", message, receiverId.ToString());
        }

    }
}
