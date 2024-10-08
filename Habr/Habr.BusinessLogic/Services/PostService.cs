using Habr.Common.DTO.Posts;
using Habr.Common.DTO.Result;
using Habr.BusinessLogic.Interfaces;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Habr.Common.Helpers.Validation;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Habr.Common.Resources;
using Serilog;
using Habr.Common.Enums;
using Habr.Common.DTO.Pagination;
using Habr.Common.Const;
using Habr.DataAccess.Extensions;
using Habr.Common.Helpers;

namespace Habr.BusinessLogic.Services
{
    public class PostService : IPostService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _dataContext;

        public PostService(IMapper mapper, DataContext dataContext)
        {
            _mapper = mapper;
            _dataContext = dataContext;
        }

        public async Task<GenericResult<int>> CreatePostAsync(PostCreateDTO dto, int userId)
        {
            Tuple<bool, string> isPostDataValid = PostValidationHelper
                .IsPostDataValid(dto.Title, dto.Text, PostConstants.PostTitleMaxLength, PostConstants.PostTextMaxLength);

            if (!isPostDataValid.Item1)
            {
                return new GenericResult<int>
                {
                    Success = false,
                    ErrorMessage = isPostDataValid.Item2.Trim()
                };
            }

            var newPost = new Post
            {
                Title = dto.Title,
                Text = dto.Text,
                Created = DateTime.UtcNow,
                UserId = userId,
                IsPublished = dto.IsPublished,
                PublicationDate = dto.IsPublished ? DateTime.UtcNow : null,
            };

            _dataContext.Posts.Add(newPost);

            await _dataContext.SaveChangesAsync();

            if (dto.IsPublished)
            {
                Log.Information(string.Format(LogMessageResource.PostPublished, userId, DateTime.UtcNow));
            }

            return new GenericResult<int>
            {
                Data = newPost.Id,
                Success = true
            };
        }

        public async Task<GenericResult<bool>> DeletePostAsync(int id, int userId, UserRole userRole)
        {
            var post = await _dataContext.Posts
                .Include(x => x.Comments)
                .Include(x => x.PostRates)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (post == null)
            {
                return new GenericResult<bool>
                {
                    ErrorMessage = PostMessageResource.PostNotFound
                };
            }

            if (post.UserId != userId && userRole != UserRole.Admin)
            {
                return new GenericResult<bool>
                {
                    ErrorMessage = PostMessageResource.PostNotFound
                };
            }

            _dataContext.Remove(post);

            await _dataContext.SaveChangesAsync();

            return new GenericResult<bool>
            {
                Data = true,
                Success = true
            };
        }

        public async Task<GenericResult<PaginatedDTO<PostListItemDTO>>> GetPublishedPostsV1Async(
            int page = PostConstants.DefaultPageNumber,
            int pageSize = PostConstants.DefaultPageSize)
        {
            var posts = _dataContext.Posts
                .Where(x => x.IsPublished)
                .ProjectTo<PostListItemDTO>(_mapper.ConfigurationProvider)
                .OrderByDescending(x => x.PublishedAt);

            var paged = await posts.PaginateAsync(pageSize, page);

            var result = new PaginatedDTO<PostListItemDTO>
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = paged.Item2,
                TotalPages = PaginationHelper.CountTotalPages(paged.Item2, pageSize),
                Items = paged.Item1
            };

            return new GenericResult<PaginatedDTO<PostListItemDTO>>
            {
                Data = result,
                Success = true
            };
        }

        public async Task<GenericResult<PaginatedDTO<PostDraftItemDTO>>> GetDraftPostsAsync(
            int userId,
            int page = PostConstants.DefaultPageNumber,
            int pageSize = PostConstants.DefaultPageSize)
        {
            var posts = _dataContext.Posts
                .Where(x => x.UserId == userId && !x.IsPublished)
                .ProjectTo<PostDraftItemDTO>(_mapper.ConfigurationProvider)
                .OrderByDescending(x => x.UpdatedAt);

            var paged = await posts.PaginateAsync(pageSize, page);

            var result = new PaginatedDTO<PostDraftItemDTO>
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = paged.Item2,
                TotalPages = PaginationHelper.CountTotalPages(paged.Item2, pageSize),
                Items = paged.Item1
            };

            return new GenericResult<PaginatedDTO<PostDraftItemDTO>>
            {
                Data = result,
                Success = true
            };
        }

        public async Task<GenericResult<PostGetDTO>> GetPostAsync(int id)
        {
            var post = await _dataContext.Posts
                .Where(x => x.Id == id && x.IsPublished)
                .ProjectTo<PostGetDTO>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();

            if (post == null)
            {
                return new GenericResult<PostGetDTO>
                {
                    ErrorMessage = PostMessageResource.PostNotFound
                };
            }

            return new GenericResult<PostGetDTO>
            {
                Data = post,
                Success = true
            };
        }

        public async Task<GenericResult<bool>> UpdatePostAsync(PostUpdateDTO dto, int userId, UserRole userRole)
        {
            Tuple<bool, string> isPostDataValid = PostValidationHelper
                .IsPostDataValid(dto.Title, dto.Text, PostConstants.PostTitleMaxLength, PostConstants.PostTextMaxLength);

            if (!isPostDataValid.Item1)
            {
                return new GenericResult<bool>
                {
                    Success = false,
                    ErrorMessage = isPostDataValid.Item2.Trim()
                };
            }

            var post = await _dataContext.Posts
                .Include(p => p.User)
                .SingleOrDefaultAsync(x => x.Id == dto.Id);

            if (post == null)
            {
                return new GenericResult<bool>
                {
                    ErrorMessage = PostMessageResource.PostNotFound
                };
            }

            if (post.UserId != userId && userRole != UserRole.Admin)
            {
                return new GenericResult<bool>
                {
                    ErrorMessage = PostMessageResource.PostNotFound
                };
            }

            if (post.IsPublished)
            {
                return new GenericResult<bool>
                {
                    ErrorMessage = PostMessageResource.PublishedPostDelete
                };
            }

            post.Title = dto.Title;
            post.Text = dto.Text;
            post.Updated = DateTime.UtcNow;

            await _dataContext.SaveChangesAsync();

            return new GenericResult<bool>
            {
                Data = true,
                Success = true
            };
        }

        public async Task<GenericResult<bool>> PublishPostAsync(int postId, int userId, UserRole userRole)
        {
            var post = await _dataContext.Posts
                .SingleOrDefaultAsync(x => x.Id == postId);

            if (post == null)
            {
                return new GenericResult<bool>
                {
                    ErrorMessage = PostMessageResource.PostNotFound
                };
            }

            if (post.UserId != userId && userRole != UserRole.Admin)
            {
                return new GenericResult<bool>
                {
                    ErrorMessage = PostMessageResource.PostNotFound
                };
            }

            if (post.IsPublished)
            {
                return new GenericResult<bool>
                {
                    Success = false,
                    ErrorMessage = PostMessageResource.PostAlreadyPublished
                };
            }

            post.IsPublished = true;
            post.PublicationDate = DateTime.UtcNow;
            post.Updated = DateTime.UtcNow;

            await _dataContext.SaveChangesAsync();

            Log.Information(string.Format(LogMessageResource.PostPublished, post.UserId, DateTime.UtcNow));

            return new GenericResult<bool>
            {
                Data = true,
                Success = true
            };
        }

        public async Task<GenericResult<bool>> DraftPostAsync(int postId, int userId, UserRole userRole)
        {
            var post = await _dataContext.Posts
                .Include(x => x.Comments)
                .SingleOrDefaultAsync(x => x.Id == postId);

            if (post == null)
            {
                return new GenericResult<bool>
                {
                    ErrorMessage = PostMessageResource.PostNotFound
                };
            }

            if (post.UserId != userId && userRole != UserRole.Admin)
            {
                return new GenericResult<bool>
                {
                    ErrorMessage = PostMessageResource.PostNotFound
                };
            }

            if (post.Comments.Count != 0 || post.Rating != null)
            {
                return new GenericResult<bool>
                {
                    ErrorMessage = PostMessageResource.PostDraftWithCommentsOrRating
                };
            }

            post.IsPublished = false;
            post.PublicationDate = null;

            await _dataContext.SaveChangesAsync();

            return new GenericResult<bool>
            {
                Data = true,
                Success = true
            };
        }

        public async Task<GenericResult<PaginatedDTO<PostListItemAuthorDTO>>> GetPublishedPostsV2Async(
            int page = PostConstants.DefaultPageNumber,
            int pageSize = PostConstants.DefaultPageSize)
        {
            var posts = _dataContext.Posts
                .Where(x => x.IsPublished)
                .ProjectTo<PostListItemAuthorDTO>(_mapper.ConfigurationProvider)
                .OrderByDescending(x => x.PublishedAt);

            var paged = await posts.PaginateAsync(pageSize, page);

            var result = new PaginatedDTO<PostListItemAuthorDTO>
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = paged.Item2,
                TotalPages = PaginationHelper.CountTotalPages(paged.Item2, pageSize),
                Items = paged.Item1
            };

            return new GenericResult<PaginatedDTO<PostListItemAuthorDTO>>
            {
                Data = result,
                Success = true
            };
        }

        public async Task<GenericResult<bool>> RatePostAsync(PostRateDTO dto, int userId)
        {
            if (dto.RatingValue <= 0 || dto.RatingValue > 5)
            {
                return new GenericResult<bool>
                {
                    ErrorMessage = CommonMessageResource.InvalidInput
                };
            }

            var post = await _dataContext.Posts
                .SingleOrDefaultAsync(x => x.Id == dto.PostId && x.IsPublished);

            if (post == null)
            {
                return new GenericResult<bool>
                {
                    ErrorMessage = PostMessageResource.PostNotFound
                };
            }

            if (post.UserId == userId)
            {
                return new GenericResult<bool>
                {
                    ErrorMessage = PostMessageResource.RateOwnPostFailed
                };
            }

            await CreateOrUpdateRateAsync(dto.RatingValue, dto.PostId, userId);

            return new GenericResult<bool>
            {
                Success = true,
                Data = true
            };
        }

        private async Task CreateOrUpdateRateAsync(int value, int postId, int userId)
        {
            var rate = await _dataContext.PostRates
                .SingleOrDefaultAsync(x => x.UserId == userId && x.PostId == postId);

            if (rate != null)
            {
                rate.Value = value;
                rate.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                var newRate = new PostRate
                {
                    PostId = postId,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    Value = value
                };

                _dataContext.Add(newRate);
            }

            await _dataContext.SaveChangesAsync();
        }
    }
}
