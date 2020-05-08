using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Data.Models
{
    /// <summary>
    /// Defines sprint model.
    /// </summary>
    [Table("Sprints")]
    public class Sprint 
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public Project Project { get; set; }
        [Required]
        public int ProjectId { get; set; }
        public List<Item> Items { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
