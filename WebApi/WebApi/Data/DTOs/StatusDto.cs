using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Data.DTOs
{
    public class StatusDto
    {
        public int Id { get; set; }

        [MaxLength(30)]
        public string Name { get; set; }

    }
}
