using Habr.Common.DTO.Comments;
using Habr.Common.DTO.Result;
using Habr.Common.Enums;

namespace Habr.BusinessLogic.Interfaces
{
    public interface ICommentService
    {
        Task<GenericResult<int>> AddCommentToPostAsync(CommentCreateDTO dto, int userId);
        Task<GenericResult<bool>> DeleteCommentAsync(int id, int userId, UserRole userRole);
        Task<GenericResult<bool>> UpdateCommentAsync(CommentUpdateDTO dto, int userId, UserRole userRole);
    }
}
