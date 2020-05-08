using System;
using System.Net;

namespace WebApi.Exceptions
{
	public abstract class ResponseExceptionBase : ApplicationException
	{
		public HttpStatusCode Code { get; }

		public ResponseExceptionBase(HttpStatusCode code, string message)
			:base(message)
		{
			this.Code = code;
		}
	}
}
