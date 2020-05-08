using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    /// <summary>
    /// Base controller class with entity cheking methods
    /// </summary>
    public abstract class LoginedUserControllerBase : ControllerBase
    {
        /// <summary>
        /// Current user id
        /// </summary>
        public virtual string UserId => User != null ? User.Claims.Single(c => c.Type == "uid").Value : "NotFound";

        /// <summary>
        /// Gets full base api url of a current controller, if annotated correctly.\
        /// </summary>
        public virtual string BaseApiUrl
        {
            get
            {
                var request = ControllerContext.HttpContext.Request;
                return request.Scheme + "://" + request.Host + request.Path;
            }
        }

        /// <summary>
        /// Is this User Administrator
        /// </summary>
        public virtual bool IsAdministrator => User.IsInRole("Administrator");

        /// <summary>
        /// Is current User Simple User
        /// </summary>
        public virtual bool IsUser => User.IsInRole("User");

        /// <summary>
        /// Is Anonymous User
        /// </summary>
        public virtual bool IsAnonymous => !User.HasClaim(c => c.Type == "uid");
    }
}