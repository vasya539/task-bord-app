using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebApi.Data.Models;

namespace WebApi.Data.DTOs
{
    /// <summary>
    /// Data transfer object for parent Item entity that contains children items.
    /// Describe model which will be return to client.
    /// </summary>
    public class ItemListDto
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

        public List<ItemDto> Items { get; set; }

        public UserDto User { get; set; }

        public ItemDto Parent { get; set; }
        public StatusDto Status { get; set; }
        public ItemTypeDto ItemType { get; set; }

    }
}