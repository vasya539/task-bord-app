using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebApi.Data.Models
{
    /// <summary>
    /// Single item to be placed on the board
    /// </summary>
    [Table("Items")]
    public class Item
    {
        #region MainProperties

        /// <summary>
        /// Primary identity key for item.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identifier of the sprint where item will be placed. Required.
        /// </summary>
        [Required]
        public int SprintId { get; set; }

        /// <summary>
        /// Identifier of the user who assigned the task. Can be NULL.
        /// </summary>
        public string AssignedUserId { get; set; }

        /// <summary>
        /// Name of the item. Required.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Short description of the item on board.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Item status describes in which column item will be placed. Required-default.
        /// </summary>
        [Required]
        public int StatusId { get; set; }

        /// <summary>
        /// Id of the item type, describe what item is that. Required-default;
        /// </summary>
        [Required]
        public int TypeId { get; set; }

        /// <summary>
        /// Variable that indicate if item is archived
        /// </summary>
        public bool IsArchived { get; set; }
        
        /// <summary>
        /// Story points to describe how many affords need to do this task
        /// </summary>
        public double? StoryPoint { get; set; }

        /// <summary>
        /// Id of parent item
        /// </summary>
        public int? ParentId { get; set; }

        #endregion MainProperties

        #region Navigation Properties

        [JsonIgnore]
        public List<Item> Items { get; set; }

        public Item Parent { get; set; }

        public ItemType ItemType { get; set; }

        public Sprint Sprint { get; set; }

        public User User { get; set; }

        public Status Status { get; set; }

        [JsonIgnore]
        public List<Comment> Comments { get; set; }

        public ICollection<ItemRelation> ItemsRelations { get; set; }
        public ICollection<ItemRelation> ItemsRelationsOf { get; set; }

        #endregion Navigation Properties
    }
}