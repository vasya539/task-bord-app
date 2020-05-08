using System.Net;

namespace WebApi.Exceptions
{
	public class ForbiddenResponseException : ResponseExceptionBase
	{
		public ForbiddenResponseException()
			:base(HttpStatusCode.Forbidden, "Access to requested resource is denied.")
		{ }

		public ForbiddenResponseException(string message)
			:base(HttpStatusCode.Forbidden, message)
		{ }
	}
}
