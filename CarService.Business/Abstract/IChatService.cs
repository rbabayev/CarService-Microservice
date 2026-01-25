using CarService.Entities.Entities;

namespace CarService.Business.Abstract
{
    public interface IChatService
    {
        Task<IEnumerable<Chat>> GetAllChatsAsync();
        Task SaveMessageAsync(Message message);
        Task<List<Message>> GetMessagesAsync(Guid userId, Guid recipientId);
        Task<Chat> GetChatByIdAsync(int id);
        Task AddChatAsync(Chat chat);
        Task UpdateChatAsync(Chat chat);
        Task DeleteChatAsync(int id);
    }
}
