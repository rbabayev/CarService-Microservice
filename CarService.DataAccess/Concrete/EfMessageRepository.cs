using CarService.DataAccess.Abstract;
using CarService.DataAccess.Database;
using CarService.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarService.DataAccess.Concrete.EntityFramework
{
    public class EfMessageRepository : IMessageRepository
    {
        private readonly CarServiceDbContext _context;

        public EfMessageRepository(CarServiceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Message>> GetAllAsync()
        {
            return await _context.Messages
                                 .Include(m => m.User)
                                 .Include(m => m.RecipientUser)
                                 .ToListAsync();
        }

        public async Task<Message> GetByIdAsync(int id)
        {
            return await _context.Messages
                                 .Include(m => m.User)
                                 .Include(m => m.RecipientUser)
                                 .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task AddAsync(Message message)
        {
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Message message)
        {
            _context.Messages.Update(message);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message != null)
            {
                _context.Messages.Remove(message);
                await _context.SaveChangesAsync();
            }
        }
    }
}
