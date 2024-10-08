using Habr.BusinessLogic.Interfaces;
using Habr.Common.DTO.Posts;
using Habr.Common.DTO.Result;
using Microsoft.AspNetCore.Mvc;

namespace Habr.WebApp.MinimalApi
{
    public static class EndpointsExtensionsV2
    {
        public static void MapPostsEndpointsV2(this IEndpointRouteBuilder routes)
        {
            var posts = routes.MapGroup("/posts-minimal").RequireAuthorization("ValidRoles");

            posts.MapGet(
                "published",
                async (IPostService service, [FromQuery] int page, [FromQuery] int pageSize) =>
                {
                    var response = await service.GetPublishedPostsV2Async(page, pageSize);
                    return response.Success ? Results.Ok(response.Data) : Results.BadRequest(response.ErrorMessage);
                })
                .Produces<GenericResult<List<PostListItemAuthorDTO>>>(StatusCodes.Status200OK)
                .Produces<GenericResult<List<PostListItemAuthorDTO>>>(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .MapToApiVersion(2)
                .WithOpenApi();
        }
    }
}
