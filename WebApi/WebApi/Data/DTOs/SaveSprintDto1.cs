using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Data.DTOs
{
    public class SaveSprintDto
    {
        [Required]
        public int ProjectId { get; set; }
        [Required][DataType("DateTime")]
        public DateTime StartDate { get; set; }
        [Required][DataType("DateTime")]
        public DateTime ExpireDate { get; set; }
    }
}
