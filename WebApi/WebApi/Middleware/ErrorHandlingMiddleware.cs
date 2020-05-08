using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

using WebApi.Exceptions;

namespace WebApi.Middleware
{
    /// <summary>
    /// Defines and configures error handling middleware.
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        private readonly IWebHostEnvironment _environment;
        private const string defaultErrorMessage = "Oops. Something went wrong. Please try again later";
        private readonly RequestDelegate next;

        /// <summary>
        /// Class constructor, initialize state of ErrorHandlingMiddleware object.
        /// </summary>
        /// <param name="next">Next element of ASP.Net Core request processing pipeline.</param>
        /// <param name="environment">Object of hosting environment.</param>
        public ErrorHandlingMiddleware(RequestDelegate next, IWebHostEnvironment environment)
        {
            this.next = next;
            _environment = environment;
        }

        /// <summary>
        /// Method that trying to invoke next element of ASP.Net Core request processing pipeline
        /// and catches unhandled exceptions.
        /// </summary>
        /// <param name="context">Instance of http context.</param>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (ResponseExceptionBase appException)
            {
                await HandleAppException(context, appException);
            }
            catch (Exception ex)
            {
                if (_environment.IsDevelopment())
                {
                    await HandleExceptionDeveloperModeAsync(context, ex);
                }
                else
                {
                    await HandleExceptionAsync(context);
                }
            }
        }

        /// <summary>
        /// Handles exception of type ResponseExceptionBase and returns response to client.
        /// </summary>
        /// <param name="context">Instance of http context.</param>
        /// <param name="e">Instance of catched exception.</param>
        /// <returns>Http response on client side.</returns>
        private static Task HandleAppException(HttpContext context, ResponseExceptionBase e)
        {
            context.Response.StatusCode = (int)e.Code;
            context.Response.ContentType = "application/json";
            string body = JsonConvert.SerializeObject(new { message = e.Message });
            return context.Response.WriteAsync(body);
        }

        /// <summary>
        /// Handles exception of any type that remains still unhandled in non development mode
        /// and returns response to client.
        /// </summary>
        /// <param name="context">Instance of http context.</param>
        /// <returns>Http response on client side.</returns>
        private static Task HandleExceptionAsync(HttpContext context)
        {
            var code = HttpStatusCode.InternalServerError;
            var result = JsonConvert.SerializeObject(new { message = defaultErrorMessage });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }

        /// <summary>
        /// Handles exception of any type that remains still unhandled in development mode
        /// and returns response to client.
        /// </summary>
        /// <param name="context">Instance of http context.</param>
        /// <param name="ex">Instance of catched exception.</param>
        /// <returns>Http response on client side.</returns>
        private static Task HandleExceptionDeveloperModeAsync(HttpContext context, Exception ex)
        {
            var code = HttpStatusCode.InternalServerError;
            var result = JsonConvert.SerializeObject(new
            {
                message = ex.Message,
                detail = ex.StackTrace,
                innerExeption = ex.InnerException != null ? ex.InnerException.Message : String.Empty
            });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}