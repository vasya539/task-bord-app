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
    [Route("api/itemtypes")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class ItemTypeController : ControllerBase
    {
        private readonly IItemTypeBl _itemTypeBL;

        public ItemTypeController(IItemTypeBl itemTypeBL)
        {
            _itemTypeBL = itemTypeBL;
        }

        // GET: api/itemtypes
        [HttpGet]
        public async Task<ActionResult<List<ItemTypeDto>>> GetAllAsync()
        {
            var itemTypes = await _itemTypeBL.GetAllAsync();
            if (itemTypes == null)
                return NotFound();
            else
                return itemTypes;
        }

        // GET: api/itemtypes/id
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetItemType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var itemType = await _itemTypeBL.Read(id);
            if (itemType == null)
                return NotFound();
            return Ok(itemType);
        }

        //PUT: api/itemtypes/
        [HttpPut]
        public async Task<ActionResult<object>> UpdateItemType([FromBody] ItemTypeDto itemTypeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                await _itemTypeBL.Update(itemTypeDto);
            }
            catch (System.Exception)
            {

                return StatusCode(500);
            }

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult<object>> CreateItemType([FromBody] ItemTypeDto itemTypeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            await _itemTypeBL.Create(itemTypeDto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<object>> DeleteItemType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                await _itemTypeBL.Delete(id);
                return Ok();
            }
            catch (System.Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
