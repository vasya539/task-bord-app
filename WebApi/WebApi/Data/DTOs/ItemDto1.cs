using System.ComponentModel.DataAnnotations;

namespace WebApi.Data.DTOs
{
    /// <summary>
    /// Data-Transfer-Object for Item entity. Describe model which will be return to client.
    /// </summary>
    public class ItemDto
    {
        public int Id { get; set; }

        [Required]
        public int SprintId { get; set; }

        public string AssignedUserId { get; set; }

        [MaxLength(30)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public int StatusId { get; set; }

        [Required]
        public int TypeId { get; set; }
    }
}