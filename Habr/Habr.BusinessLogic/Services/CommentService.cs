using Habr.Common.DTO.Comments;
using Habr.Common.DTO.Result;
using Habr.BusinessLogic.Interfaces;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Habr.Common.Resources;
using Habr.Common.Enums;

namespace Habr.BusinessLogic.Services
{
    public class CommentService : ICommentService
    {
        private readonly DataContext _dataContext;

        public CommentService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<GenericResult<int>> AddCommentToPostAsync(CommentCreateDTO dto, int userId)
        {
            if (string.IsNullOrEmpty(dto.Text))
            {
                return new GenericResult<int>
                {
                    ErrorMessage = CommonMessageResource.InvalidInput
                };
            }

            var post = await _dataContext.Posts.SingleOrDefaultAsync(x => x.Id == dto.PostId);

            if (post == null || !post.IsPublished)
            {
                return new GenericResult<int>
                {
                    ErrorMessage = CommentMessageResource.CommentToUnpublishedPost
                };
            }

            if (dto.ParentCommentId != null)
            {
                var parentComment = await _dataContext.Comments.SingleOrDefaultAsync(x => x.Id == dto.ParentCommentId);

                if (parentComment == null || parentComment.PostId != dto.PostId)
                {
                    return new GenericResult<int>
                    {
                        ErrorMessage = CommentMessageResource.ParentCommentNotFound
                    };
                }
            }

            var newComment = new Comment
            {
                Text = dto.Text,
                Created = DateTime.UtcNow,
                UserId = userId,
                PostId = dto.PostId,
                ParentCommentId = dto.ParentCommentId
            };

            _dataContext.Comments.Add(newComment);

            await _dataContext.SaveChangesAsync();

            return new GenericResult<int>
            {
                Data = newComment.Id,
                Success = true
            };
        }

        public async Task<GenericResult<bool>> DeleteCommentAsync(int id, int userId, UserRole userRole)
        {
            var comment = await _dataContext.Comments
                .Include(x => x.Replies)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (comment == null)
            {
                return new GenericResult<bool>
                {
                    ErrorMessage = CommentMessageResource.CommentNotFound
                };
            }

            if (comment.UserId != userId && userRole != UserRole.Admin)
            {
                return new GenericResult<bool>
                {
                    ErrorMessage = CommentMessageResource.CommentNotFound
                };
            }

            if (comment.Replies.Count > 0)
            {
                return new GenericResult<bool>
                {
                    ErrorMessage = CommentMessageResource.CommentWithRepliesDelete
                };
            }

            _dataContext.Comments.Remove(comment);

            await _dataContext.SaveChangesAsync();

            return new GenericResult<bool>
            {
                Data = true,
                Success = true
            };
        }

        public async Task<GenericResult<bool>> UpdateCommentAsync(CommentUpdateDTO dto, int userId, UserRole userRole)
        {
            var comment = await _dataContext.Comments
                .SingleOrDefaultAsync(x => x.Id == dto.Id);

            if (comment == null)
            {
                return new GenericResult<bool>
                {
                    ErrorMessage = CommentMessageResource.CommentNotFound
                };
            }

            if (comment.UserId != userId && userRole != UserRole.Admin)
            {
                return new GenericResult<bool>
                {
                    ErrorMessage = CommentMessageResource.CommentNotFound
                };
            }

            comment.Text = dto.Text;

            await _dataContext.SaveChangesAsync();

            return new GenericResult<bool>
            {
                Data = true,
                Success = true
            };
        }
    }
}
