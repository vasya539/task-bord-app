using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data.Models;

namespace WebApi.Data.DTOs
{
    public class SprintDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public List<Item> Items { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
