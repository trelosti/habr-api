using Habr.Common.DTO.Auth;
using Habr.Common.DTO.Comments;
using Habr.Common.DTO.Posts;
using Habr.Common.DTO.Result;
using Habr.BusinessLogic.Interfaces;
using Habr.BusinessLogic.Services;
using Habr.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using Habr.Common.Enums;

public class Program
{
    private static int userId = 1;
    private static UserRole userRole = UserRole.User;

    private static readonly IEnumerable<string> commands = new HashSet<string>
    {
        "register",
        "login",
        "create post",
        "read post",
        "read posts",
        "update post",
        "delete post",
        "read draft posts",
        "create comment",
        "reply to comment",
        "delete comment",
        "publish post",
        "draft post"
    };

    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        await ApplyMigrationsAsync(host);
        await RunInputLoopAsync(host);
        await host.RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddDbContext<DataContext>(options =>
                    options.UseSqlServer(hostContext.Configuration.GetConnectionString("DataContext")));

                services.AddScoped<IUserService, UserService>();
                services.AddScoped<IPostService, PostService>();
                services.AddScoped<ICommentService, CommentService>();
            });

    private static async Task ApplyMigrationsAsync(IHost host)
    {
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<DataContext>();
            await context.Database.MigrateAsync();
        }
    }

    private static async Task RunInputLoopAsync(IHost host)
    {
        Console.WriteLine("The list of commands:");

        foreach (var item in commands)
        {
            Console.WriteLine($"{item}");
        }

        while (true)
        {
            Console.WriteLine("Enter a command:");
            var input = Console.ReadLine().Trim().ToLower();

            if (!commands.Contains(input))
            {
                Console.WriteLine("The command not recognized, try again");
                continue;
            }

            var result = await ProcessMainCommandAsync(input, host);
            Console.WriteLine(JsonSerializer.Serialize(result));
        }
    }

    private static async Task<object> ProcessMainCommandAsync(string input, IHost host)
    {
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            var userService = services.GetRequiredService<IUserService>();
            var postService = services.GetRequiredService<IPostService>();
            var commentService = services.GetRequiredService<ICommentService>();

            if (input == "register")
            {
                Console.WriteLine("Enter <email>|<password>");
                var dto = Console.ReadLine().Split('|');
                Console.WriteLine("Enter name (optional)");
                var name = Console.ReadLine().Trim();

                var result = await userService.RegisterAsync(new RegisterDTO
                {
                    Name = name,
                    Email = dto[0],
                    Password = dto[1]
                });

                return result;
            }
            else if (input == "login")
            {
                Console.WriteLine("Enter <email>|<password>");
                var dto = Console.ReadLine().Trim().Split('|');

                var result = await userService.LoginAsync(new LoginDTO
                {
                    Email = dto[0],
                    Password = dto[1]
                });

                return result;
            }
            else
            {
                if (userId == 0)
                {
                    Console.WriteLine("Register or log in");
                    return new GenericResult<string>
                    {
                        ErrorMessage = "The user is not logged in"
                    };
                }

                if (input.Contains("post"))
                {
                    var commandResult = await ProcessPostCommandAsync(input, postService);

                    return commandResult;
                }

                if (input.Contains("comment"))
                {
                    var commandResult = await ProcessCommentCommandAsync(input, commentService);

                    return commandResult;
                }
            }

            await Task.CompletedTask;
        }

        return new { Success = false };
    }

    private static async Task<object> ProcessPostCommandAsync(string input, IPostService postService)
    {
        if (input == "create post")
        {
            Console.WriteLine("Enter <title>|<text>|<is_published (0 or 1)>");
            var dto = Console.ReadLine().Trim().Split('|');

            var result = await postService.CreatePostAsync(new PostCreateDTO
            {
                Title = dto[0],
                Text = dto[1],
                IsPublished = int.Parse(dto[2]) == 1 ? true : false
            }, userId);

            return result;
        }

        if (input == "read posts")
        {
            var result = await postService.GetPublishedPostsV1Async();

            return result;
        }

        if (input == "read draft posts")
        {
            var result = await postService.GetDraftPostsAsync(userId);

            return result;
        }

        if (input == "read post")
        {
            Console.WriteLine("Enter <id>");
            var id = int.Parse(Console.ReadLine().Trim());

            var result = await postService.GetPostAsync(id);

            return result;
        }

        if (input == "update post")
        {
            Console.WriteLine("Enter <id>|<title>|<text>");
            var dto = Console.ReadLine().Trim().Split('|');

            var result = await postService.UpdatePostAsync(new PostUpdateDTO
            {
                Id = int.Parse(dto[0]),
                Title = dto[1],
                Text = dto[2],
            }, userId, userRole);

            return result;
        }

        if (input == "delete post")
        {
            Console.WriteLine("Enter <id>");
            var id = int.Parse(Console.ReadLine().Trim());

            var result = await postService.DeletePostAsync(id, userId, userRole);

            return result;
        }

        if (input == "publish post")
        {
            Console.WriteLine("Enter <id>");
            var id = int.Parse(Console.ReadLine().Trim());

            var result = await postService.PublishPostAsync(id, userId, userRole);

            return result;
        }

        if (input == "draft post")
        {
            Console.WriteLine("Enter <id>");
            var id = int.Parse(Console.ReadLine().Trim());

            var result = await postService.DraftPostAsync(id, userId, userRole);

            return result;
        }

        return new { Success = false };
    }

    private static async Task<object> ProcessCommentCommandAsync(string input, ICommentService commentService)
    {
        if (input == "create comment")
        {
            Console.WriteLine("Enter <parent_id>(not required)|<post_id>|<text>");
            var dto = Console.ReadLine().Trim().Split('|');

            var result = await commentService.AddCommentToPostAsync(new CommentCreateDTO
            {
                ParentCommentId = string.IsNullOrEmpty(dto[0]) ? null : int.Parse(dto[0]),
                PostId = int.Parse(dto[1]),
                Text = dto[2],
            }, userId);

            return result;
        }

        if (input == "delete comment")
        {
            Console.WriteLine("Enter <id>");
            var id = int.Parse(Console.ReadLine().Trim());

            var result = await commentService.DeleteCommentAsync(id, userId, userRole);

            return result;
        }

        return new { Success = false };
    }
}
