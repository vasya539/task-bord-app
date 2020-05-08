using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data.Models;

namespace WebApi.Data.DTOs
{
    public class ProjectUserDto
    {
        public int ProjectId { get; set; }
        public string UserId { get; set; }
        public int UserRoleId { get; set; }

        //public virtual ProjectDto Project { get; set; }
        public UserDto User { get; set; }
    }
}
