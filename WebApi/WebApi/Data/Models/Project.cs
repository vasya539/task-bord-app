using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Data.Models
{
    /// <summary>
    /// Single project to be placed on the page
    /// </summary>
    [Table("Projects")]
    public class Project
    {
        public Project()
        {
            ProjectsUsers = new HashSet<ProjectUser>();
            Sprints = new HashSet<Sprint>();
        }

        /// <summary>
        /// Primary identity key for project.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the project.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the project. 
        /// </summary>
        public string Description { get; set; }

        public virtual ICollection<ProjectUser> ProjectsUsers { get; set; }
        public virtual ICollection<Sprint> Sprints { get; set; }
    }
}

