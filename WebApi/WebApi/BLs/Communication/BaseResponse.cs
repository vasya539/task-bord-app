namespace WebApi.BLs.Communication
{
    /// <summary>
    /// Base class for responses which will be sent from business logic to controller.
    /// </summary>
    public abstract class BaseResponse
    {
        public bool Success { get; protected set; }
        public string Message { get; protected set; }

        /// <summary>
        /// Base construtor for initializing response status and message.
        /// </summary>
        /// <param name="success">State of operation.</param>
        /// <param name="message">Message which will be returned to controller.</param>
        public BaseResponse(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
