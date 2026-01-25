using CarService.DataAccess.Abstract;
using CarService.DataAccess.Database;
using CarService.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarService.DataAccess.Concrete.EntityFramework
{
    public class EfChatRepository : IChatRepository
    {
        private readonly CarServiceDbContext _context;

        public EfChatRepository(CarServiceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Chat>> GetAllAsync()
        {
            return await _context.Chats
                                 .Include(c => c.Receiver)
                                 .Include(c => c.Messages)
                                 .ToListAsync();
        }

        public async Task<Chat> GetByIdAsync(int id)
        {
            return await _context.Chats
                                 .Include(c => c.Receiver)
                                 .Include(c => c.Messages)
                                 .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(Chat chat)
        {
            await _context.Chats.AddAsync(chat);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Chat chat)
        {
            _context.Chats.Update(chat);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var chat = await _context.Chats.FindAsync(id);
            if (chat != null)
            {
                _context.Chats.Remove(chat);
                await _context.SaveChangesAsync();
            }
        }
    }
}
