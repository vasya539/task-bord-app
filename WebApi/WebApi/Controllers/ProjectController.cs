using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApi.Data.DTOs;
using WebApi.BLs.Interfaces;
using Microsoft.AspNetCore.Authorization;
using WebApi.BLs.Communication;
using WebApi.Extensions;

namespace WebApi.Controllers
{
    /// <summary>
    ///This controller created to get requests from server and do operations with items in database.
    /// </summary>
    [EnableCors]
    [Route("api/projects")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class ProjectController : LoginedUserControllerBase
    {
        private readonly IProjectBl _projectBL;

        /// <summary>
        /// Constructor to initialize projectBl
        /// </summary>
        /// <param name="projectBl">Class with project business logic</param>
        public ProjectController(IProjectBl projectBl)
        {
            _projectBL = projectBl;
        }

        /// <summary>
        /// Get all projects from database
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var projects = await _projectBL.GetAllAsync();
            if (projects == null)
                return NotFound();
            else
                return Ok(projects);
        }

        /// <summary>
        /// Get specific project from database
        /// </summary>
        /// <param name="id">Id of the project to be returned to client</param>
        /// <returns>ProjectDto object</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject([FromRoute] int id)
        {
            var project = await _projectBL.GetByIdAsync(id, UserId);
            if (project == null)
                return NotFound();
            else
                return Ok(project);
        }

        /// <summary>
        /// Create new project in database.
        /// </summary>
        /// <param name="project">Project from response-body</param>
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] ProjectDto project)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrorMessages());
            ProjectResponse result = await _projectBL.CreateAsync(project, UserId);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok();
        }

        /// <summary>
        /// Update specific project in database.
        /// </summary>
        /// <param name="project">Project from response-body</param>
        [EnableCors]
        [HttpPut]
        public async Task<IActionResult> UpdateProject([FromBody] ProjectDto project)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrorMessages());

           ProjectResponse result = await _projectBL.UpdateAsync(project, UserId);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok();
        }

        /// <summary>
        /// Delete specific project from database.
        /// </summary>
        /// <param name="id">Project from response-body</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject([FromRoute] int id)
        {
            ProjectResponse result = await _projectBL.DeleteAsync(id, UserId);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok();
        }
    }
}
