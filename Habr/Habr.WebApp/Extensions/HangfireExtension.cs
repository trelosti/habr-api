using Habr.BusinessLogic.Interfaces;
using Hangfire;

namespace Habr.WebApp.Extensions
{
    public static class HangfireExtension
    {
        public static WebApplicationBuilder ConfigureHangfire(this WebApplicationBuilder builder)
        {
            builder.Services.AddHangfire(x =>
            {
                x.UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DataContext"));
            });

            builder.Services.AddHangfireServer();

            return builder;
        }

        public static void StartRecurringJobs(this IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices;

            using (var scope = serviceProvider.CreateScope())
            {
                var jobService = scope.ServiceProvider.GetRequiredService<IRatingService>();

                IRecurringJobManager recurringJobManager = serviceProvider.GetRequiredService<IRecurringJobManager>();

                recurringJobManager.AddOrUpdate(
                    "ComputePostsRatingsJob",
                    () => jobService.ComputePostsRatingsAsync(),
                    Cron.Daily(0 ,00),
                    new RecurringJobOptions
                    {
                        TimeZone = TimeZoneInfo.Utc
                    });
            }
        }
    }
}
