using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Data.Models
{
    [Table("ProjectsUsers")]
    public class ProjectUser
    {
        public int ProjectId { get; set; }
        public string UserId { get; set; }

        [Column("UserRole")]
        public int UserRoleId { get; set; }

        public virtual Project Project { get; set; }
        public virtual User User { get; set; }
        public AppUserRole UserRole { get; set; }
    }
}
