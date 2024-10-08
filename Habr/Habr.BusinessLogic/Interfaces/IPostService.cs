using Habr.Common.Const;
using Habr.Common.DTO.Pagination;
using Habr.Common.DTO.Posts;
using Habr.Common.DTO.Result;
using Habr.Common.Enums;

namespace Habr.BusinessLogic.Interfaces
{
    public interface IPostService
    {
        Task<GenericResult<int>> CreatePostAsync(PostCreateDTO dto, int userId);
        Task<GenericResult<PostGetDTO>> GetPostAsync(int id);
        Task<GenericResult<bool>> UpdatePostAsync(PostUpdateDTO dto, int userId, UserRole userRole);
        Task<GenericResult<bool>> DeletePostAsync(int id, int userId, UserRole userRole);
        Task<GenericResult<bool>> PublishPostAsync(int postId, int userId, UserRole userRole);
        Task<GenericResult<bool>> DraftPostAsync(int postId, int userId, UserRole userRole);
        Task<GenericResult<bool>> RatePostAsync(PostRateDTO dto, int userId);
        Task<GenericResult<PaginatedDTO<PostListItemDTO>>> GetPublishedPostsV1Async(
            int page = PostConstants.DefaultPageNumber,
            int pageSize = PostConstants.DefaultPageSize);
        Task<GenericResult<PaginatedDTO<PostListItemAuthorDTO>>> GetPublishedPostsV2Async(
            int page = PostConstants.DefaultPageNumber,
            int pageSize = PostConstants.DefaultPageSize);
        Task<GenericResult<PaginatedDTO<PostDraftItemDTO>>> GetDraftPostsAsync(
            int userId,
            int page = PostConstants.DefaultPageNumber,
            int pageSize = PostConstants.DefaultPageSize);
    }
}
