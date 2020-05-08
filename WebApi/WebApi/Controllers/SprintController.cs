using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApi.Data.DTOs;
using WebApi.BLs.Interfaces;
using WebApi.Extensions;
using WebApi.BLs.Communication;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers
{
    /// <summary>
    /// Gets access to operations of managing sprints in project.
    /// </summary>
    [Authorize(Roles = "User")]
    [EnableCors]
    [Route("api/sprints")]
    [ApiController]
    public class SprintController : LoginedUserControllerBase
    {
        private readonly ISprintBl _sprintBl;

        /// <summary>
        /// Constructor for controller, initializes sprint business logic.
        /// </summary>
        /// <param name="sprintBl">Object of sprint business logic.</param>
        public SprintController(ISprintBl sprintBl)
        {
            _sprintBl = sprintBl;
        }

        /// <summary>
        /// Resceives http request and calls BL method, that gets all sprints by project Id.
        /// </summary>
        /// <param name="projectId">Id of project.</param>
        /// <returns>Collection of sprint DTO's.</returns>
        [HttpGet("/api/projects/{projectId}/sprints")]
        public async Task<IEnumerable<SprintDto>> GetAllByProjectIdAsync(int projectId)
        {
            IEnumerable<SprintDto> dto = await _sprintBl.GetAllByProjectIdAsync(projectId, UserId);
            return dto;
        }

        /// <summary>
        /// Resceives http request and calls BL method, that gets all items by sprint Id.
        /// </summary>
        /// <param name="id">Id of sprint.</param>
        /// <returns>Collection of ItemList DTO's, grouped by parent item.</returns>
        [HttpGet("{id}/items")]
        public async Task<ActionResult> GetAllSprintItemsAsync(int id)
        {
            var allItems = await _sprintBl.GetAllSprintItemsAsync(id, UserId);
            if (allItems == null)
                return NotFound();
            else
                return Ok(allItems);
        }

        /// <summary>
        /// Resceives http request and calls BL method, that gets sprint by Id.
        /// </summary>
        /// <param name="id">Id of sprint.</param>
        /// <returns>Sprint DTO.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            SprintDto sprintDTO = await _sprintBl.GetByIdAsync(id, UserId);
            if (sprintDTO == null)
                return BadRequest("Sprint does not exist");
            return Ok(sprintDTO);
        }

        /// <summary>
        /// Resceives http request and calls BL method, that creates a sprint.
        /// </summary>
        /// <param name="dto">Sprint DTO.</param>
        /// <returns>Status code of operation.</returns>
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] SprintDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrorMessages());
            SprintResponse result = await _sprintBl.CreateAsync(dto, UserId);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok();
        }

        /// <summary>
        /// Resceives http request and calls BL method, that updates a sprint.
        /// </summary>
        /// <param name="dto">Sprint DTO.</param>
        /// <returns>Status code of operation.</returns>
        [HttpPut]
        public async Task<IActionResult> PutAsync([FromBody] SprintDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrorMessages());

            SprintResponse result = await _sprintBl.UpdateAsync(dto, UserId);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok();
        }

        /// <summary>
        /// Resceives http request and calls BL method, that deletes a sprint.
        /// </summary>
        /// <param name="id">Id of sprint.</param>
        /// <returns>Status code of operation.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            SprintResponse result = await _sprintBl.DeleteAsync(id, UserId);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok();
        }
    }
}