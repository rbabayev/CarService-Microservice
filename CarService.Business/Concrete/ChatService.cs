using CarService.Business.Abstract;
using CarService.DataAccess.Abstract;
using CarService.Entities.Entities;

namespace CarService.Business.Concrete
{
    public class ChatService : IChatService
    {
        private readonly IMessageRepository _messageRepo;
        private readonly IChatRepository _chatRepository;

        public ChatService(IChatRepository chatRepository, IMessageRepository messageRepo)
        {
            _chatRepository = chatRepository;
            _messageRepo = messageRepo;
        }

        public async Task<IEnumerable<Chat>> GetAllChatsAsync()
        {
            return await _chatRepository.GetAllAsync();
        }



        public async Task<Chat> GetChatByIdAsync(int id)
        {
            return await _chatRepository.GetByIdAsync(id);
        }

        public async Task AddChatAsync(Chat chat)
        {
            await _chatRepository.AddAsync(chat);
        }

        public async Task SaveMessageAsync(Message message)
        {
            await _messageRepo.AddAsync(message);
        }

        public async Task<List<Message>> GetMessagesAsync(Guid userId, Guid recipientId)
        {
            var allMessages = await _messageRepo.GetAllAsync();
            return allMessages
                .Where(m =>
                    (m.UserId == userId && m.RecipientUserId == recipientId) ||
                    (m.UserId == recipientId && m.RecipientUserId == userId))
                .OrderBy(m => m.DateTime)
                .ToList();
        }
        public async Task UpdateChatAsync(Chat chat)
        {
            await _chatRepository.UpdateAsync(chat);
        }

        public async Task DeleteChatAsync(int id)
        {
            await _chatRepository.DeleteAsync(id);
        }
    }
}
