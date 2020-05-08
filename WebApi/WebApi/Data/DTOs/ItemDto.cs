using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebApi.Data.Models;

namespace WebApi.Data.DTOs
{
    /// <summary>
    /// Data-Transfer-Object for Item entity. Describe model which will be return to client.
    /// </summary>
    public class ItemDto
    {
        public int Id { get; set; }

        public int? ParentId { get; set; }

        public int SprintId { get; set; }

        public string AssignedUserId { get; set; }

        [MaxLength(30)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public int StatusId { get; set; }

        [Required]
        public int TypeId { get; set; }

        public bool IsArchived { get; set; }
        public double? StoryPoint { get; set; }
        public UserDto User { get; set; }

        public ItemDto Parent { get; set; }
        public StatusDto Status { get; set; }
        public ItemTypeDto ItemType { get; set; }

        public override bool Equals(object obj)
        {
            ItemDto item = (ItemDto)obj;
            return ((item.Id == this.Id) &&
                (item.Name == this.Name) &&
                (item.ParentId == this.ParentId) &&
                (item.SprintId == this.SprintId) &&
                (item.AssignedUserId == this.AssignedUserId) &&
                (item.Description == this.Description) &&
                (item.StatusId == this.StatusId) &&
                (item.TypeId == this.TypeId) &&
                (item.IsArchived == this.IsArchived) &&
                (item.StoryPoint == this.StoryPoint));
        }
    }
}