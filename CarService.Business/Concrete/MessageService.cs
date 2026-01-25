using CarService.Business.Abstract;
using CarService.DataAccess.Abstract;
using CarService.Entities.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarService.Business.Concrete
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;

        public MessageService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task<IEnumerable<Message>> GetAllMessagesAsync()
        {
            return await _messageRepository.GetAllAsync();
        }

        public async Task<Message> GetMessageByIdAsync(int id)
        {
            return await _messageRepository.GetByIdAsync(id);
        }

        public async Task AddMessageAsync(Message message)
        {
            await _messageRepository.AddAsync(message);
        }

        public async Task UpdateMessageAsync(Message message)
        {
            await _messageRepository.UpdateAsync(message);
        }

        public async Task DeleteMessageAsync(int id)
        {
            await _messageRepository.DeleteAsync(id);
        }
    }
}
