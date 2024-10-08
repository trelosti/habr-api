using Asp.Versioning;
using Habr.BusinessLogic.Interfaces;
using Habr.Common.DTO.Comments;
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
    public class CommentsController : BaseController
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CommentCreateDTO commentCreateDTO)
        {
            var response = await _commentService.AddCommentToPostAsync(commentCreateDTO, base.CurrentUserId);

            if (!response.Success)
            {
                return BadRequest(response.ErrorMessage);
            }

            return Ok(response.Data);
        }

        [HttpDelete("{commentId}")]
        public async Task<IActionResult> Delete([FromRoute] int commentId)
        {
            var response =  await _commentService.DeleteCommentAsync(commentId, base.CurrentUserId, base.CurrentUserRole);

            if (!response.Success)
            {
                return BadRequest(response.ErrorMessage);
            }

            return NoContent();
        }

        [HttpPut("{commentId}")]
        public async Task<IActionResult> Update([FromBody] CommentUpdateDTO dto)
        {
            var response = await _commentService.UpdateCommentAsync(dto, base.CurrentUserId, base.CurrentUserRole);

            if (!response.Success)
            {
                return BadRequest(response.ErrorMessage);
            }

            return Ok(response.Data);
        }
    }
}
