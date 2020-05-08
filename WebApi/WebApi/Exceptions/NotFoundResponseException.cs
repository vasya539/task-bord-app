using System.Net;

namespace WebApi.Exceptions
{
	public class NotFoundResponseException : ResponseExceptionBase
	{
		public NotFoundResponseException()
			: base(HttpStatusCode.NotFound, "Not Found.")
		{ }

		public NotFoundResponseException(string message)
			:base(HttpStatusCode.NotFound, message)
		{ }
	}
}
