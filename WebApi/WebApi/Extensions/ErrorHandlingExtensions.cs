using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using WebApi.Middleware;

namespace WebApi.Extensions
{
    public static class ErrorHandlingExtensions
    {
        public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder, IWebHostEnvironment environment)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>(environment);
        }

    }
}
