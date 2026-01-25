using CarService.Entities.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarService.Business.Abstract
{
    public interface IMessageService
    {
        Task<IEnumerable<Message>> GetAllMessagesAsync();
        Task<Message> GetMessageByIdAsync(int id);
        Task AddMessageAsync(Message message);
        Task UpdateMessageAsync(Message message);
        Task DeleteMessageAsync(int id);
    }
}
