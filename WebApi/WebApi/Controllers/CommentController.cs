using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.BLs.Interfaces;
using WebApi.Data.DTOs;
using WebApi.Data.Models;

namespace WebApi.Controllers
{
    /// <summary>
    ///This controller created to get requests from server and do operations with comments in database.
    /// </summary>
    [EnableCors]
    [Route("api/comments")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class CommentController : LoginedUserControllerBase
    {
        private readonly ICommentBl _commentBl;

        /// <summary>
        /// Constructor to initialize CommentBL
        /// </summary>
        /// <param name="commentBl">Class with comment business logic</param>
        public CommentController(ICommentBl commentBl)
        {
            _commentBl = commentBl;
        }


        /// <summary>
        /// Get all comments from database
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> GetAllAsync()
        {
            var allComments = await _commentBl.GetAllAsync();
            if (allComments == null)
                return NotFound();
            return Ok(allComments);
        }

        /// <summary>
        /// Get specific comment from database
        /// </summary>
        /// <param name="id">Id of comment</param>
        /// <returns>CommentDto object</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult> ReadAsync([FromRoute] int id)
        {
            var comment = await _commentBl.ReadAsync(id);
            if (comment == null)
                return NotFound();
            return Ok(comment);
        }

        /// <summary>
        /// Create new comment in database
        /// </summary>
        /// <param name="comment"></param>
        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] CommentDto comment)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _commentBl.CreateAsync(comment);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(result.Message);
        }

        /// <summary>
        /// Update specific comment in database
        /// </summary>
        /// <param name="comment"></param>
        [EnableCors]
        [HttpPut]
        public async Task<ActionResult> UpdateAsync([FromBody] CommentDto comment)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _commentBl.UpdateAsync(comment);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(result.Message);
        }
        
        /// <summary>
        /// Delete specific comment from database
        /// </summary>
        /// <param name="id">Comment id</param>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _commentBl.DeleteAsync(id, UserId);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(result.Message);
        }
    }
}