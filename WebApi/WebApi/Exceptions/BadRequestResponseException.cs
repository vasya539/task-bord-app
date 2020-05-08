using System.Net;

namespace WebApi.Exceptions
{
	public class BadRequestResponseException : ResponseExceptionBase
	{
		public BadRequestResponseException()
			:base(HttpStatusCode.BadRequest, "Bad Request")
		{ }

		public BadRequestResponseException(string message)
			:base(HttpStatusCode.BadRequest, message)
		{ }
	}
}
