using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Data.DTOs
{
    /// <summary>
    /// Data-Transfer-Object for Project entity. Describe model which will be return to client.
    /// </summary>
    public class ProjectDto
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<ProjectUserDto> ProjectsUsers { get; set; }
        public virtual ICollection<SprintDto> Sprints { get; set; }
    }
}
