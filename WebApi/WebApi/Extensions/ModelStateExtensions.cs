using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;

namespace WebApi.Extensions
{
    /// <summary>
    /// Extension class which add a method to ModelStateDictionary.
    /// </summary>
    public static class ModelStateExtensions
    {
        /// <summary>
        /// Creates collection with all validation error messages of instance.
        /// </summary>
        /// <param name="dictionary">Extends ModelStateDictionary class.</param>
        /// <returns>Collection with all validation error messages of instance.</returns>
        public static List<string> GetErrorMessages(this ModelStateDictionary dictionary)
        {
            return dictionary.SelectMany(m => m.Value.Errors)
                             .Select(m => m.ErrorMessage)
                             .ToList();
        }
    }
}
