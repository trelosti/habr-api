using Habr.BusinessLogic.Interfaces;
using Habr.Common.DTO.Auth;
using Habr.Common.DTO.Comments;
using Habr.Common.DTO.Posts;
using Habr.Common.DTO.Result;
using Habr.WebApp.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Habr.WebApp.MinimalApi
{
    public static class EndpointsExtensionsV1
    {
        public static void MapUsersEndpointsV1(this IEndpointRouteBuilder routes)
        {
            var users = routes.MapGroup("/users-minimal");

            users.MapPost(
                "register",
                async (IUserService service, [FromBody] RegisterDTO dto) =>
                {
                    var response = await service.RegisterAsync(dto);
                    return response.Success ? Results.Ok(response.Data) : Results.BadRequest(response.ErrorMessage);
                })
                .Produces<GenericResult<string>>(StatusCodes.Status200OK)
                .Produces<GenericResult<string>>(StatusCodes.Status400BadRequest)
                .WithOpenApi();

            users.MapPost(
                "login",
                async (IUserService service, [FromBody] LoginDTO dto) =>
                {
                    var response = await service.LoginAsync(dto);
                    return response.Success ? Results.Ok(response.Data) : Results.BadRequest(response.ErrorMessage);
                })
                .Produces<GenericResult<string>>(StatusCodes.Status200OK)
                .Produces<GenericResult<string>>(StatusCodes.Status400BadRequest)
                .WithOpenApi();

            users.MapGet(
                string.Empty,
                async (IUserService service, HttpContext context) =>
                {
                    var response = await service.GetUserInfo(context.GetUserId());
                    return response.Success ? Results.Ok(response.Data) : Results.BadRequest(response.ErrorMessage);
                })
                .RequireAuthorization("ValidRoles")
                .Produces<GenericResult<string>>(StatusCodes.Status200OK)
                .Produces<GenericResult<string>>(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .WithOpenApi();

            users.MapPost(
                "refreshToken",
                async (IUserService service, [FromBody] RefreshTokenDTO dto) =>
                {
                    var response = await service.RefreshToken(dto);
                    return response.Success ? Results.Ok(response.Data) : Results.BadRequest(response.ErrorMessage);
                })
                .Produces<GenericResult<AuthTokenDTO>>(StatusCodes.Status200OK)
                .Produces<GenericResult<AuthTokenDTO>>(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .WithOpenApi();
        }

        public static void MapPostsEndpointsV1(this IEndpointRouteBuilder routes)
        {
            var posts = routes.MapGroup("/posts-minimal").RequireAuthorization("ValidRoles");

            posts.MapGet(
                "{id}",
                async (IPostService service, [FromRoute] int id) =>
                {
                    var response = await service.GetPostAsync(id);
                    return response.Success ? Results.Ok(response.Data) : Results.BadRequest(response.ErrorMessage);
                })
                .Produces<GenericResult<PostGetDTO>>(StatusCodes.Status200OK)
                .Produces<GenericResult<PostGetDTO>>(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .WithOpenApi();

            posts.MapGet(
                "published",
                async (IPostService service, [FromQuery] int page, [FromQuery] int pageSize) =>
                {
                    var response = await service.GetPublishedPostsV1Async(page, pageSize);
                    return response.Success ? Results.Ok(response.Data) : Results.BadRequest(response.ErrorMessage);
                })
                .Produces<GenericResult<List<PostListItemDTO>>>(StatusCodes.Status200OK)
                .Produces<GenericResult<List<PostListItemDTO>>>(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .MapToApiVersion(1)
                .WithOpenApi();

            posts.MapGet(
                "drafts",
                async (IPostService service, HttpContext context, [FromQuery] int page, [FromQuery] int pageSize) =>
                {
                    var response = await service.GetDraftPostsAsync(context.GetUserId(), page, pageSize);
                    return response.Success ? Results.Ok(response.Data) : Results.BadRequest(response.ErrorMessage);
                })
                .Produces<GenericResult<List<PostDraftItemDTO>>>(StatusCodes.Status200OK)
                .Produces<GenericResult<List<PostDraftItemDTO>>>(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .WithOpenApi();

            posts.MapPost(
                string.Empty,
                async (IPostService service, [FromBody] PostCreateDTO dto, HttpContext context) =>
                {
                    var response = await service.CreatePostAsync(dto, context.GetUserId());
                    return response.Success ? Results.Ok(response.Data) : Results.BadRequest(response.ErrorMessage);
                })
                .Produces<GenericResult<int>>(StatusCodes.Status200OK)
                .Produces<GenericResult<int>>(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .WithOpenApi();

            posts.MapPut(
                string.Empty,
                async (IPostService service, [FromBody] PostUpdateDTO dto, HttpContext context) =>
                {
                    var response = await service.UpdatePostAsync(dto, context.GetUserId(), context.GetUserRole());
                    return response.Success ? Results.Ok(response.Data) : Results.BadRequest(response.ErrorMessage);
                })
                .Produces<GenericResult<int>>(StatusCodes.Status200OK)
                .Produces<GenericResult<int>>(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .WithOpenApi();

            posts.MapDelete(
                "{id}",
                async (IPostService service, [FromRoute] int id, HttpContext context) =>
                {
                    var response = await service.DeletePostAsync(id, context.GetUserId(), context.GetUserRole());
                    return response.Success ? Results.NoContent() : Results.BadRequest(response.ErrorMessage);
                })
                .Produces(StatusCodes.Status204NoContent)
                .Produces<GenericResult<bool>>(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .WithOpenApi();

            posts.MapPost(
                "{id}/publish",
                async (IPostService service, [FromRoute] int id, HttpContext context) =>
                {
                    var response = await service.PublishPostAsync(id, context.GetUserId(), context.GetUserRole());
                    return response.Success ? Results.Ok(response.Data) : Results.BadRequest(response.ErrorMessage);
                })
                .Produces<GenericResult<bool>>(StatusCodes.Status200OK)
                .Produces<GenericResult<bool>>(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .WithOpenApi();

            posts.MapPost(
                "{id}/moveToDraft",
                async (IPostService service, [FromRoute] int id, HttpContext context) =>
                {
                    var response = await service.DraftPostAsync(id, context.GetUserId(), context.GetUserRole());
                    return response.Success ? Results.Ok(response.Data) : Results.BadRequest(response.ErrorMessage);
                })
                .Produces<GenericResult<bool>>(StatusCodes.Status200OK)
                .Produces<GenericResult<bool>>(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .WithOpenApi();

            posts.MapPost(
                "rate",
                async (IPostService service, [FromBody] PostRateDTO dto, HttpContext context) =>
                {
                    var response = await service.RatePostAsync(dto, context.GetUserId());
                    return response.Success ? Results.Ok(response.Data) : Results.BadRequest(response.ErrorMessage);
                })
                .Produces<GenericResult<bool>>(StatusCodes.Status200OK)
                .Produces<GenericResult<bool>>(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .WithOpenApi();
        }

        public static void MapCommentsEndpointsV1(this IEndpointRouteBuilder routes)
        {
            var comments = routes.MapGroup("/comments-minimal").RequireAuthorization("ValidRoles");

            comments.MapPost(
                string.Empty,
                async (ICommentService service, [FromBody] CommentCreateDTO dto, HttpContext context) =>
                {
                    var response = await service.AddCommentToPostAsync(dto, context.GetUserId());
                    return response.Success ? Results.Ok(response.Data) : Results.BadRequest(response.ErrorMessage);
                })
                .Produces<GenericResult<int>>(StatusCodes.Status200OK)
                .Produces<GenericResult<int>>(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .WithOpenApi();

            comments.MapDelete(
                "{id}",
                async (ICommentService service, [FromRoute] int id, HttpContext context) =>
                {
                    var response = await service.DeleteCommentAsync(id, context.GetUserId(), context.GetUserRole());
                    return response.Success ? Results.NoContent() : Results.BadRequest(response.ErrorMessage);
                })
                .Produces(StatusCodes.Status204NoContent)
                .Produces<GenericResult<bool>>(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .WithOpenApi();

            comments.MapPut(
                "{id}",
                async (ICommentService service, [FromBody] CommentUpdateDTO dto, HttpContext context) =>
                {
                    var response = await service.UpdateCommentAsync(dto, context.GetUserId(), context.GetUserRole());
                    return response.Success ? Results.Ok(response.Data) : Results.BadRequest(response.ErrorMessage);
                })
                .Produces(StatusCodes.Status200OK)
                .Produces<GenericResult<bool>>(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .WithOpenApi();
        }
    }
}
