using System;
using System.Collections.Generic;
using WebApi.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Data.DTOs
{
    /// <summary>
    /// Data transfer object for Sprint entity. Describe model which will be return to client.
    /// </summary>
    public class SprintDto
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int ProjectId { get; set; }
        public List<Item> Items { get; set; }    
        [Required]
        [DataType("DateTime")]
        public DateTime StartDate { get; set; }
        [Required]
        [DataType("DateTime")]
        public DateTime EndDate { get; set; }
    }
}
