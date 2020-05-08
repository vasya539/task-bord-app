using WebApi.Data.DTOs;

namespace WebApi.BLs.Communication
{
    /// <summary>
    /// Class for response which will be sent to controller from business logic.
    /// </summary>
    public class SprintResponse : BaseResponse
    {
        public SprintDto SprintDTO { get; private set; }

        /// <summary>
        /// Private constructor which initialize SprintDto field.
        /// </summary>
        /// <param name="success">State of operation.</param>
        /// <param name="message">Message which will be returned to controller.</param>
        /// <param name="sprintDTO">Sprint which will be returned to controller.</param>
        private SprintResponse(bool success, string message, SprintDto sprintDTO) : base(success, message)
        {
            SprintDTO = sprintDTO;
        }

        /// <summary>
        /// Constructor for response of succeeded action.
        /// </summary>
        /// <param name="sprintDTO">Sprint which will be returned to controller.</param>
        public SprintResponse(SprintDto sprintDTO) : this(true, string.Empty, sprintDTO)
        { }

        /// <summary>
        /// Constructor for response of faulted action.
        /// </summary>
        /// <param name="message">Message which will be returned to controller.</param>
        public SprintResponse(string message) : this(false, message, null)
        { }
    }
}
