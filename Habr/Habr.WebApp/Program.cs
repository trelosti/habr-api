using Asp.Versioning.ApiExplorer;
using Habr.Common.DTO.Auth;
using Habr.Common.Exceptions;
using Habr.Common.Exceptions.Mappers;
using Habr.DataAccess;
using Habr.WebApp.Extensions;
using Habr.WebApp.OpenApi;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DataContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DataContext")));

#region Logging
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Services.AddSerilog();
builder.Host.UseSerilog(Log.Logger);
#endregion

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization();

builder.Services.Configure<JwtConfigurationDTO>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddSingleton<IExceptionMapper, ExceptionToProblemDetailsMapper>();
builder.Services.AddAutoMapper(typeof(Habr.BusinessLogic.AssemblyMarker).Assembly);

builder.Services.AddExceptionHandler<DefaultGlobalExceptionHandler>();

builder.ConfigureHangfire();

builder.RegisterServices();

builder.Services.AddEndpointsApiExplorer();

builder.ConfigureVersioning();
builder.ConfigureSwagger();
builder.ConfigureAuthentication();

builder.Services.ConfigureOptions<ConfigureSwaggerGenOptions>();

var app = builder.Build();

await app.ConfigureAppAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        IReadOnlyList<ApiVersionDescription> descriptions = app.DescribeApiVersions();

        foreach(var description in descriptions)
        {
            string url = $"/swagger/{description.GroupName}/swagger.json";
            string name = description.GroupName.ToUpperInvariant();

            options.SwaggerEndpoint(url, name);
        }
    });
}

app.UseExceptionHandler(_ => { });
app.StartRecurringJobs();

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseHangfireDashboard();

app.Run();
