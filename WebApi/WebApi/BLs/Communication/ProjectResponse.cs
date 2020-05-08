using WebApi.Data.DTOs;

namespace WebApi.BLs.Communication
{
    /// <summary>
    /// Class for Response which will be send to client from controllers. Describe the status of operations.
    /// </summary>
    public class ProjectResponse : BaseResponse
    {
        /// <summary>
        /// Instance of ProjectDto which can be returned to user with answer if it's need. 
        /// </summary>
        public ProjectDto ProjectDTO { get; private set; }

        /// <summary>
        /// Constructor which initialize ProjectDto field.
        /// </summary>
        /// <param name="success">State of operation</param>
        /// <param name="message">Message which will be returned to client</param>
        /// <param name="projectDto">Project which will be returned to client</param>
        private ProjectResponse(bool success, string message, ProjectDto projectDto) : base(success, message)
        {
            ProjectDTO = projectDto;
        }

        /// <summary>
        /// Second constructor if we want to return only ProjectDto.
        /// </summary>
        /// <param name="projectDto"></param>
        public ProjectResponse(ProjectDto projectDto) : this(true, string.Empty, projectDto)
        { }

        /// <summary>
        /// Third constructor if we want to return only message and state, without ProjectDto.
        /// </summary>
        /// <param name="message">Message which will be returned to client</param>
        public ProjectResponse(string message) : this(false, message, null)
        { }
    }
}
