using Asp.Versioning;
using Habr.BusinessLogic.Interfaces;
using Habr.Common.DTO.Posts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Habr.WebApp.Controllers
{
    [ApiVersion(1)]
    [ApiVersion(2)]
    [Route("api/v{v:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(Policy = "ValidRoles")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PostsController : BaseController
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            var response = await _postService.GetPostAsync(id);

            if (!response.Success)
            {
                return BadRequest(response.ErrorMessage);
            }

            return Ok(response.Data);
        }

        [MapToApiVersion(1)]
        [HttpGet("published")]
        public async Task<IActionResult> GetV1([FromQuery] int page, [FromQuery] int pageSize)
        {
            var response = await _postService.GetPublishedPostsV1Async(page, pageSize);

            if (!response.Success)
            {
                return BadRequest(response.ErrorMessage);
            }

            return Ok(response.Data);
        }

        [MapToApiVersion(2)]
        [HttpGet("published")]
        public async Task<IActionResult> GetV2([FromQuery] int page, [FromQuery] int pageSize)
        {
            var response = await _postService.GetPublishedPostsV2Async(page, pageSize);

            if (!response.Success)
            {
                return BadRequest(response.ErrorMessage);
            }

            return Ok(response.Data);
        }

        [HttpGet("drafts")]
        public async Task<IActionResult> GetDrafts([FromQuery] int page, [FromQuery] int pageSize)
        {
            var response = await _postService.GetDraftPostsAsync(base.CurrentUserId, page, pageSize);

            if (!response.Success)
            {
                return BadRequest(response.ErrorMessage);
            }

            return Ok(response.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PostCreateDTO postCreateDTO)
        {
            var response = await _postService.CreatePostAsync(postCreateDTO, base.CurrentUserId);

            if (!response.Success)
            {
                return BadRequest(response.ErrorMessage);
            }

            return Ok(response.Data);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] PostUpdateDTO postUpdateDTO)
        {
            var response = await _postService.UpdatePostAsync(postUpdateDTO, base.CurrentUserId, base.CurrentUserRole);

            if (!response.Success)
            {
                return BadRequest(response.ErrorMessage);
            }

            return Ok(response.Data);
        }

        [HttpDelete("{postId}")]
        public async Task<IActionResult> Delete([FromRoute] int postId)
        {
            var response = await _postService.DeletePostAsync(postId, base.CurrentUserId, base.CurrentUserRole);

            if (!response.Success)
            {
                return BadRequest(response.ErrorMessage);
            }

            return NoContent();
        }

        [HttpPost("{postId}/publish")]
        public async Task<IActionResult> Publish([FromRoute] int postId)
        {
            var response = await _postService.PublishPostAsync(postId, base.CurrentUserId, CurrentUserRole);

            if (!response.Success)
            {
                return BadRequest(response.ErrorMessage);
            }

            return Ok(response.Data);
        }

        [HttpPost("{postId}/moveToDraft")]
        public async Task<IActionResult> Draft([FromRoute] int postId)
        {
            var response = await _postService.DraftPostAsync(postId, base.CurrentUserId, CurrentUserRole);

            if (!response.Success)
            {
                return BadRequest(response.ErrorMessage);
            }

            return Ok(response.Data);
        }

        [HttpPost("rate")]
        public async Task<IActionResult> Rate([FromBody] PostRateDTO dto)
        {
            var response = await _postService.RatePostAsync(dto, base.CurrentUserId);

            if (!response.Success)
            {
                return BadRequest(response.ErrorMessage);
            }

            return Ok(response.Data);
        }
    }
}
