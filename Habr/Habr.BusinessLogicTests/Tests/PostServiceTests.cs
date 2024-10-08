using Habr.Common.Const;
using Habr.Common.DTO.Auth;
using Habr.Common.DTO.Posts;
using Habr.Common.Enums;
using Habr.Common.Resources;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Habr.BusinessLogicTests.Tests
{
    public class PostServiceTests
    {
        private const int userId = 1;
        private const UserRole userRole = UserRole.User;

        [Fact]
        public async Task CreatePostAsync_EmptyTitle_ReturnsSuccessFalse()
        {
            // Arrange 
            var dto = new PostCreateDTO { Title = "", Text = "text", IsPublished = true };

            using (var context = TestHelper.GetDataContext())
            {
                var postService = TestHelper.GetPostService(context);

                // Act
                var result = await postService.CreatePostAsync(dto, userId);

                // Assert
                Assert.False(result.Success);
                Assert.Equal(result.ErrorMessage, PostMessageResource.TitleRequired);
            }
        }

        [Fact]
        public async Task CreatePostAsync_TooLongTitle_ReturnsSuccessFalse()
        {
            // Arrange 
            var dto = new PostCreateDTO { Title = RepeatString("a", PostConstants.PostTitleMaxLength + 1), Text = "text", IsPublished = true };

            using (var context = TestHelper.GetDataContext())
            {
                var postService = TestHelper.GetPostService(context);

                // Act
                var result = await postService.CreatePostAsync(dto, userId);

                // Assert
                Assert.False(result.Success);
                Assert.Equal(result.ErrorMessage, PostMessageResource.TitleMaxLength);
            }
        }

        [Fact]
        public async Task CreatePostAsync_EmptyText_ReturnsSuccessFalse()
        {
            // Arrange 
            var dto = new PostCreateDTO { Title = "title", Text = "", IsPublished = true };

            using (var context = TestHelper.GetDataContext())
            {
                var postService = TestHelper.GetPostService(context);

                // Act
                var result = await postService.CreatePostAsync(dto, userId);

                // Assert
                Assert.False(result.Success);
                Assert.Equal(result.ErrorMessage, PostMessageResource.TextRequired);
            }
        }

        [Fact]
        public async Task CreatePostAsync_TooLongText_ReturnsSuccessFalse()
        {
            // Arrange 
            var dto = new PostCreateDTO { Title = "title", Text = RepeatString("a", PostConstants.PostTextMaxLength + 1), IsPublished = true };

            using (var context = TestHelper.GetDataContext())
            {
                var postService = TestHelper.GetPostService(context);

                // Act
                var result = await postService.CreatePostAsync(dto, userId);

                // Assert
                Assert.False(result.Success);
                Assert.Equal(result.ErrorMessage, PostMessageResource.TextMaxLength);
            }
        }

        [Fact]
        public async Task CreatePostAsync_ValidOnePostData_ReturnsSuccessTrueAndSingleTrue()
        {
            // Arrange 
            var dto = new PostCreateDTO { Title = "title", Text = "text", IsPublished = true };

            using (var context = TestHelper.GetDataContext())
            {
                var postService = TestHelper.GetPostService(context);

                // Act
                var result = await postService.CreatePostAsync(dto, userId);

                var posts = context.Posts;

                // Assert
                Assert.True(result.Success);
                Assert.Single(posts);
            }
        }

        [Fact]
        public async Task CreatePostAsync_ValidTwoPostData_ReturnsSuccessTrueAndCountTwo()
        {
            // Arrange 
            var dto = new PostCreateDTO { Title = "title", Text = "text", IsPublished = true };

            using (var context = TestHelper.GetDataContext())
            {
                var postService = TestHelper.GetPostService(context);

                // Act
                var resultFirst = await postService.CreatePostAsync(dto, userId);
                var resultSecond = await postService.CreatePostAsync(dto, userId);

                var postsCount = context.Posts.Count();

                // Assert
                Assert.True(resultFirst.Success);
                Assert.True(resultSecond.Success);
                Assert.Equal(2, postsCount);
            }
        }

        [Fact]
        public async Task DeletePostAsync_ValidPostIdAndUserId_ReturnsSuccessTrueAndCountEmpty()
        {
            // Arrange 
            var dto = new PostCreateDTO { Title = "title", Text = "text", IsPublished = true };

            using (var context = TestHelper.GetDataContext())
            {
                var postService = TestHelper.GetPostService(context);

                var createResult = await postService.CreatePostAsync(dto, userId);

                // Act
                var deleteResult = await postService.DeletePostAsync(createResult.Data, userId, userRole);

                var posts = context.Posts;

                // Assert
                Assert.True(deleteResult.Success);
                Assert.Empty(posts);
            }
        }

        [Fact]
        public async Task DeletePostAsync_InvalidPostId_ReturnsSuccessFalseAndCountNotEmpty()
        {
            // Arrange 
            var dto = new PostCreateDTO { Title = "title", Text = "text", IsPublished = true };

            using (var context = TestHelper.GetDataContext())
            {
                var postService = TestHelper.GetPostService(context);

                var createResult = await postService.CreatePostAsync(dto, userId);

                // Act
                var deleteResult = await postService.DeletePostAsync(0, userId, userRole);

                var posts = context.Posts;

                // Assert
                Assert.False(deleteResult.Success);
                Assert.NotEmpty(posts);
            }
        }

        [Fact]
        public async Task GetPublishedPostsAsync_ValidTwoPostData_ReturnsSuccessTrue()
        {
            // Arrange 
            var dto = new PostCreateDTO { Title = "title", Text = "text", IsPublished = true };

            using (var context = TestHelper.GetDataContext())
            {
                var postService = TestHelper.GetPostService(context);

                await postService.CreatePostAsync(dto, userId);
                await postService.CreatePostAsync(dto, userId);

                // Act
                var result = await postService.GetPublishedPostsV1Async();

                var unpublishedPosts = context.Posts.Where(x => !x.IsPublished);

                // Assert
                Assert.True(result.Success);
                Assert.Equal(2, result.Data.Items.Count());
                Assert.Empty(unpublishedPosts);
            }
        }

        [Fact]
        public async Task GetDraftPostsAsync_ValidTwoPostData_ReturnsSuccessTrue()
        {
            // Arrange 
            var dto = new PostCreateDTO { Title = "title", Text = "text", IsPublished = false };

            using (var context = TestHelper.GetDataContext())
            {
                var postService = TestHelper.GetPostService(context);

                await postService.CreatePostAsync(dto, userId);
                await postService.CreatePostAsync(dto, userId);

                // Act
                var result = await postService.GetDraftPostsAsync(userId);

                var publishedPosts = context.Posts.Where(x => x.IsPublished);

                // Assert
                Assert.True(result.Success);
                Assert.Equal(2, result.Data.Items.Count());
                Assert.Empty(publishedPosts);
            }
        }

        [Fact]
        public async Task GetPostAsync_ValidOnePublishedPostData_ReturnsSuccessTrue()
        {
            // Arrange 
            var dto = new PostCreateDTO { Title = "title", Text = "text", IsPublished = true };

            using (var context = TestHelper.GetDataContext())
            {
                var postService = TestHelper.GetPostService(context);

                var createResult = await postService.CreatePostAsync(dto, userId);

                // Act
                var result = await postService.GetPostAsync(createResult.Data);

                // Assert
                Assert.True(result.Success);
                Assert.NotNull(result.Data);
                Assert.Equal(createResult.Data, result.Data.Id);
            }
        }

        [Fact]
        public async Task GetPostAsync_ValidOneDraftPostData_ReturnsSuccessFalse()
        {
            // Arrange 
            var dto = new PostCreateDTO { Title = "title", Text = "text", IsPublished = false };

            using (var context = TestHelper.GetDataContext())
            {
                var postService = TestHelper.GetPostService(context);

                var createResult = await postService.CreatePostAsync(dto, userId);

                // Act
                var result = await postService.GetPostAsync(createResult.Data);

                // Assert
                Assert.False(result.Success);
                Assert.Null(result.Data);
            }
        }

        [Fact]
        public async Task UpdatePostAsync_ValidUpdatePostData_ReturnsSuccessTrue()
        {
            // Arrange 
            var postCreateDto = new PostCreateDTO { Title = "title", Text = "text", IsPublished = true };
            var registerUserDto = new RegisterDTO { Email = "user@example.com", Name = "user", Password = "user123" };

            using (var context = TestHelper.GetDataContext())
            {
                var postService = TestHelper.GetPostService(context);
                var userService = TestHelper.GetUserService(context);

                var registerUser = await userService.RegisterAsync(registerUserDto);
                var createResult = await postService.CreatePostAsync(postCreateDto, userId);

                var post = await postService.GetPostAsync(createResult.Data);

                var postUpdateDto = new PostUpdateDTO { Id = post.Data.Id, Title = "newTitle", Text = "newText" };

                await postService.DraftPostAsync(post.Data.Id, userId, userRole);

                // Act
                var updateResult = await postService.UpdatePostAsync(postUpdateDto, userId, userRole);

                await postService.PublishPostAsync(post.Data.Id, userId, userRole);
                var updatedPost = await postService.GetPostAsync(createResult.Data);

                // Assert
                Assert.True(updateResult.Success);
                Assert.NotNull(updatedPost.Data);
                Assert.Equal(updatedPost.Data.Title, postUpdateDto.Title);
                Assert.Equal(updatedPost.Data.Text, postUpdateDto.Text);
            }
        }

        [Fact]
        public async Task UpdatePostAsync_UpdatePublishedPostData_ReturnsSuccessFalse()
        {
            // Arrange 
            var postCreateDto = new PostCreateDTO { Title = "title", Text = "text", IsPublished = true };
            var registerUserDto = new RegisterDTO { Email = "user@example.com", Name = "user", Password = "user123" };

            using (var context = TestHelper.GetDataContext())
            {
                var postService = TestHelper.GetPostService(context);
                var userService = TestHelper.GetUserService(context);

                var registerUser = await userService.RegisterAsync(registerUserDto);
                var createResult = await postService.CreatePostAsync(postCreateDto, userId);

                var post = await postService.GetPostAsync(createResult.Data);

                var postUpdateDto = new PostUpdateDTO { Id = post.Data.Id, Title = "newTitle", Text = "newText" };

                // Act
                var updateResult = await postService.UpdatePostAsync(postUpdateDto, userId, userRole);

                // Assert
                Assert.False(updateResult.Success);
            }
        }

        [Fact]
        public async Task PublishPostAsync_ValidPostIdAndUserId_ReturnsIsPublishedTrue()
        {
            // Arrange 
            var dto = new PostCreateDTO { Title = "title", Text = "text", IsPublished = false };

            using (var context = TestHelper.GetDataContext())
            {
                var postService = TestHelper.GetPostService(context);

                var createResult = await postService.CreatePostAsync(dto, userId);
                var createdPost = await context.Posts.SingleOrDefaultAsync(x => x.Id == createResult.Data);

                // Act
                await postService.PublishPostAsync(createResult.Data, userId, userRole);

                // Assert
                Assert.True(createdPost?.IsPublished);
            }
        }

        [Fact]
        public async Task PublishPostAsync_InvalidPostId_ReturnsIsPublishedFalse()
        {
            // Arrange 
            var dto = new PostCreateDTO { Title = "title", Text = "text", IsPublished = false };

            using (var context = TestHelper.GetDataContext())
            {
                var postService = TestHelper.GetPostService(context);

                var createResult = await postService.CreatePostAsync(dto, userId);
                var createdPost = await context.Posts.SingleOrDefaultAsync(x => x.Id == createResult.Data);

                // Act
                await postService.PublishPostAsync(0, userId, userRole);

                // Assert
                Assert.False(createdPost?.IsPublished);
            }
        }

        [Fact]
        public async Task DraftPostAsync_ValidPostIdAndUserId_ReturnsIsPublishedFalse()
        {
            // Arrange 
            var dto = new PostCreateDTO { Title = "title", Text = "text", IsPublished = true };

            using (var context = TestHelper.GetDataContext())
            {
                var postService = TestHelper.GetPostService(context);

                var createResult = await postService.CreatePostAsync(dto, userId);
                var createdPost = await context.Posts.SingleOrDefaultAsync(x => x.Id == createResult.Data);

                // Act
                await postService.DraftPostAsync(createResult.Data, userId, userRole);

                // Asser
                Assert.False(createdPost?.IsPublished);
            }
        }

        [Fact]
        public async Task DraftPostAsync_InvalidPostId_ReturnsIsPublishedTrue()
        {
            // Arrange 
            var dto = new PostCreateDTO { Title = "title", Text = "text", IsPublished = true };

            using (var context = TestHelper.GetDataContext())
            {
                var postService = TestHelper.GetPostService(context);

                var createResult = await postService.CreatePostAsync(dto, userId);
                var createdPost = await context.Posts.SingleOrDefaultAsync(x => x.Id == createResult.Data);

                // Act
                await postService.DraftPostAsync(0, userId, userRole);

                // Assert
                Assert.True(createdPost?.IsPublished);
            }
        }

        private static string RepeatString(string value, int count)
        {
            return new StringBuilder(value.Length * count).Insert(0, value, count).ToString();
        }
    }
}
