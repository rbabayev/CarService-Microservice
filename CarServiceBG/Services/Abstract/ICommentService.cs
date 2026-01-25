using CarServiceBG.DTOs;

namespace CarServiceBG.Services.Abstract
{
    public interface ICommentService
    {
        Task<CommentResponseDto> CreateAsync(CommentDto dto);
        Task<List<CommentResponseDto>> GetByWorkerAsync(Guid workerId);
    }
}
