using AutoMapper;
using Habr.BusinessLogic.Interfaces;
using Habr.BusinessLogic.Services;
using Habr.Common.DTO.Auth;
using Habr.Common.DTO.Comments;
using Habr.Common.DTO.Posts;
using Habr.Common.DTO.Users;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;

namespace Habr.BusinessLogicTests
{
    public static class TestHelper
    {
        public static DataContext GetDataContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            return new DataContext(optionsBuilder);
        }

        public static IPostService GetPostService(DataContext context)
        {
            var mockMapper = new Mock<IMapper>();

            mockMapper.Setup(x => x.ConfigurationProvider)
                   .Returns(
                       () => new MapperConfiguration(
                           cfg =>
                           {
                               cfg.CreateMap<Post, PostListItemDTO>();
                               cfg.CreateMap<Post, PostGetDTO>();
                               cfg.CreateMap<Post, PostDraftItemDTO>();
                               cfg.CreateMap<Comment, CommentGetDTO>();
                               cfg.CreateMap<User, UserInfoDTO>();
                           }));

            return new PostService(mockMapper.Object, context);
        }

        public static IUserService GetUserService(DataContext context)
        {
            var mockMapper = new Mock<IMapper>();
            var mockJwtService = new Mock<IJwtService>();

            IOptions<JwtConfigurationDTO> jwtConfiguration = Options.Create<JwtConfigurationDTO>(new JwtConfigurationDTO());
            jwtConfiguration.Value.RefreshExpiresInDays = "7";

            return new UserService(context, mockJwtService.Object, mockMapper.Object, jwtConfiguration);
        }
    }
}
