using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using WebApi.BLs.Interfaces;
using WebApi.Data.DTOs;
using WebApi.Data.Models;
using WebApi.Interfaces.IRepositories;
using WebApi.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers
{
    [EnableCors("AllowAllOrigin")]
    [Route("api/statuses")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class StatusController : ControllerBase
    {
        private readonly IStatusBl _statusBL;

        public StatusController(IStatusBl statusBL)
        {
            _statusBL = statusBL;
        }

        // GET: api/statuses
        [HttpGet]
        public async Task<ActionResult<List<StatusDto>>> GetAllAsync()
        {
            var statuses = await _statusBL.GetAllAsync();
            if (statuses == null)
                return NotFound();
            else
                return statuses;
        }

        // GET: api/statuses/id
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetStatus([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var status = await _statusBL.Read(id);
            if (status == null)
                return NotFound();
            return Ok(status);
        }

        //PUT: api/statuses/
        [HttpPut]
        public async Task<ActionResult<object>> UpdateStatus([FromBody] StatusDto statusDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                await _statusBL.Update(statusDto);
            }
            catch (System.Exception)
            {

                return StatusCode(500);
            }

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult<object>> CreateStatus([FromBody] StatusDto statusDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            await _statusBL.Create(statusDto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<object>> DeleteStatus([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                await _statusBL.Delete(id);
                return Ok();
            }
            catch (System.Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
