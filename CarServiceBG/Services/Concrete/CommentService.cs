using AutoMapper;
using CarService.DataAccess.Database;
using CarService.Entities.Entities;
using CarServiceBG.DTOs;
using CarServiceBG.Services.Abstract;
using Microsoft.EntityFrameworkCore;

namespace CarServiceBG.Services.Concrete
{
    public class CommentService : ICommentService
    {
        private readonly CarServiceDbContext _context;
        private readonly IMapper _mapper;

        public CommentService(CarServiceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CommentResponseDto> CreateAsync(CommentDto dto)
        {
            var comment = _mapper.Map<Comment>(dto);
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();

            return _mapper.Map<CommentResponseDto>(comment);
        }

        public async Task<List<CommentResponseDto>> GetByWorkerAsync(Guid workerId)
        {
            var comments = await _context.Comments
                .Where(c => c.WorkerId == workerId)
                .OrderByDescending(c => c.DateTime)
                .ToListAsync();

            return _mapper.Map<List<CommentResponseDto>>(comments);
        }
    }
}
