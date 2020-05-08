using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApi.BLs.Interfaces;
using WebApi.Data.DTOs;

namespace WebApi.Controllers
{
    /// <summary>
    ///This controller created to get requests from server and do operations with items in database.
    /// </summary>
    [EnableCors]
    [Route("api/items")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class ItemController : LoginedUserControllerBase
    {
        private readonly IItemBl _itemBl;
        private readonly ICommentBl _commentBl;
        private readonly IItemRelationBl _itemRelationBl;

        /// <summary>
        /// Constructor to initialize itemBl, commentBl, and relationBL
        /// </summary>
        /// <param name="itemBl">Class with item business logic</param>
        /// <param name="commentBl">Class with comment business logic</param>
        /// <param name="relationBl">Class with ItemRelation business logic</param>
        public ItemController(IItemBl itemBl, ICommentBl commentBl, IItemRelationBl relationBl)
        {
            _itemBl = itemBl;
            _commentBl = commentBl;
            _itemRelationBl = relationBl;
        }

        /// <summary>
        /// Get all items from database
        /// </summary>
        /// <returns>List of ItemDto</returns>
        [HttpGet]
        public async Task<ActionResult> GetAllAsync()
        {
            var allItems = await _itemBl.GetAllAsync();
            if (allItems == null)
                return NotFound();
            return Ok(allItems);
        }

        /// <summary>
        /// Get all item by specific sprint
        /// </summary>
        /// <param name="sprintId">Sprint's id</param>
        /// <returns>List of ItemDto</returns>
        [HttpGet]
        [Route("sprints/{sprintId}")]
        public async Task<ActionResult> GetAllBySprintIdAsync([FromRoute] int sprintId)
        {
            var allItems = await _itemBl.GetBySprintIdAsync(sprintId);
            if (allItems == null)
                return NotFound();
            return Ok(allItems);
        }

        /// <summary>
        /// Get all archived items by for specific sprint
        /// </summary>
        /// <param name="sprintId">Id of specific sprint</param>
        /// <returns>List of ItemDto</returns>
        [HttpGet]
        [Route("sprints/{sprintId}/archived")]
        public async Task<ActionResult> GetArchivedBySprintIdAsync([FromRoute] int sprintId)
        {
            var allItems = await _itemBl.GetArchivedBySprintIdAsync(sprintId);
            if (allItems == null)
                return NotFound();
            return Ok(allItems);
        }

        /// <summary>
        /// Get specific item from database
        /// </summary>
        /// <param name="id">Id of the item to be returned to client</param>
        /// <returns>ItemDto object</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult> GetItemAsync([FromRoute] int id)
        {
            var item = await _itemBl.ReadAsync(id);
            if (item == null)
                return NotFound();
            return Ok(item);
        }

        /// <summary>
        /// Create new item in database.
        /// </summary>
        /// <param name="item">Item from response-body</param>
        [HttpPost]
        public async Task<ActionResult> CreateItemAsync([FromBody] ItemDto item)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _itemBl.CreateAsync(item, UserId);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(result.Message);
        }

        /// <summary>
        /// Update specific item in database.
        /// </summary>
        /// <param name="item">Item from response-body</param>
        [EnableCors]
        [HttpPut]
        public async Task<ActionResult> UpdateItemAsync([FromBody] ItemDto item)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _itemBl.UpdateAsync(item, UserId);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(result.Message);
        }

        /// <summary>
        /// Delete specific item from database.
        /// </summary>
        /// <param name="id">Item from response-body</param>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteItemAsync([FromRoute] int id)
        {
            var result = await _itemBl.DeleteAsync(id, UserId);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(result.Message);
        }

        /// <summary>
        /// Archive specific item on board
        /// </summary>
        /// <param name="id">Id of item to be archived</param>
        [HttpDelete("{id}/archive")]
        public async Task<ActionResult> ArchiveItemAsync([FromRoute] int id)
        {
            var result = await _itemBl.ArchivingAsync(id, UserId);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(result.Message);
        }
 
        /// <summary>
        /// Get all comments for specific item
        /// </summary>
        /// <param name="id">Item id</param>
        [HttpGet]
        [Route("{id}/comments")]
        public async Task<ActionResult> GetAllCommentsByItemIdAsync([FromRoute] int id)
        {
            var allComments = await _commentBl.GetByItemIdAsync(id);
            if (allComments == null)
                return NotFound();
            return Ok(allComments);
        }

        /// <summary>
        /// Get all child for specific item
        /// </summary>
        /// <param name="id">Id of item parent</param>
        /// <returns>List of ItemDto</returns>
        [HttpGet]
        [Route("{id}/childs")]
        public async Task<ActionResult> GetAllChildAsync([FromRoute] int id)
        {
            var child = await _itemBl.GetAllChildAsync(id);
            if (child == null)
                return NotFound();
            return Ok(child);
        }

        /// <summary>
        /// Get all child with specific status for specific item
        /// </summary>
        /// <param name="id">Item parent id</param>
        /// <param name="statusId">Item status id</param>
        /// <returns>List of ItemDto</returns>
        [HttpGet]
        [Route("{id}/childs/statuses/{statusId}")]
        public async Task<ActionResult> GetChildWithSpecificStatusAsync([FromRoute] int id, [FromRoute] int statusId)
        {
            var child = await _itemBl.GetChildWithSpecificStatusAsync(id, statusId);
            if (child == null)
                return NotFound();
            return Ok(child);
        }

        /// <summary>
        /// Get all unparented items for specific sprint
        /// </summary>
        /// <param name="id">Sprint id</param>
        [HttpGet]
        [Route("{id}/null/childs")]
        public async Task<ActionResult> GetAllUnparentedAsync([FromRoute] int id)
        {
            var child = await _itemBl.GetUnparentedAsync(id);
            if (child == null)
                return NotFound();
            return Ok(child);
        }

        /// <summary>
        /// Get all user-stories for specific sprint
        /// </summary>
        /// <param name="id">Sprint id</param>
        [HttpGet]
        [Route("{id}/stories")]
        public async Task<ActionResult> GetAllUserStoriesAsync([FromRoute] int id)
        {
            var allItems = await _itemBl.GetUserStoriesAsync(id);
            if (allItems == null)
                return NotFound();
            return Ok(allItems);
        }

        /// <summary>
        /// Create new relation between two items
        /// </summary>
        /// <param name="firstId">First item to be related</param>
        /// <param name="secondId">Second item to be related</param>
        [HttpPost]
        [Route("{firstId}/relation/{secondId}")]
        public async Task<ActionResult> RelateItemsAsync([FromRoute] int firstId, int secondId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _itemRelationBl.CreateRecordAsync(firstId, secondId, UserId);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(result.Message);
        }

        /// <summary>
        /// Get all related items for specific item
        /// </summary>
        /// <param name="itemId">Item id</param>
        [HttpGet]
        [Route("{itemId}/relation")]
        public async Task<ActionResult> GetRelatedItemsAsync(int itemId)
        {
            var result = await _itemRelationBl.GetRelatedItemsAsync(itemId);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// Delete relation between two items
        /// </summary>
        /// <param name="firstId">First item of relation</param>
        /// <param name="secondId">Second item of relation</param>
        [HttpDelete]
        [Route("{firstId}/relation/{secondId}")]
        public async Task<ActionResult> DeleteRelationBetweenItemsAsync([FromRoute] int firstId, int secondId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _itemRelationBl.DeleteRecordAsync(firstId, secondId, UserId);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(result.Message);
        }
    }
}