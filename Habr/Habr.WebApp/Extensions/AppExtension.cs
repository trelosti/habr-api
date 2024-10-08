using Asp.Versioning;
using Habr.DataAccess;
using Habr.WebApp.MinimalApi;
using Microsoft.EntityFrameworkCore;

namespace Habr.WebApp.Extensions
{
    public static class AppExtension
    {
        public static async Task<WebApplication> ConfigureAppAsync(this WebApplication app)
        {
            var apiVersionSet = app.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1))
                .HasApiVersion(new ApiVersion(2))
                .ReportApiVersions()
                .Build();

            var versionGroup = app.MapGroup("/api/v{apiVersion:apiVersion}").WithApiVersionSet(apiVersionSet);

            MapEndpointsV1(versionGroup);
            MapEndpointsV2(versionGroup);

            await MigrateDatabaseAsync(app);

            return app;
        }

        private static async Task MigrateDatabaseAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            await dbContext.Database.MigrateAsync();
        }

        private static void MapEndpointsV1(RouteGroupBuilder versionGroupBuilder)
        {
            versionGroupBuilder.MapUsersEndpointsV1();
            versionGroupBuilder.MapPostsEndpointsV1();
            versionGroupBuilder.MapCommentsEndpointsV1();
        }

        private static void MapEndpointsV2(RouteGroupBuilder versionGroupBuilder)
        {
            versionGroupBuilder.MapPostsEndpointsV2();
        }
    }
}
