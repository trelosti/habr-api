using Habr.BusinessLogic.Interfaces;
using Habr.BusinessLogic.Services;
using Hangfire;

namespace Habr.WebApp.Extensions
{
    public static class ServiceRegisterExtension
    {
        public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IPostService, PostService>();
            builder.Services.AddScoped<ICommentService, CommentService>();
            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<IRatingService, RatingService>();

            return builder;
        }
    }
}
