using System.Collections.Generic;
using Microsoft.AspNet.Mvc.ModelBinding;
using System.Linq;

namespace AllReady.Extensions
{
    public static class ModelStateExtensions
    {
        public static List<string> GetErrorMessages(this ModelStateDictionary pModelState)
        {
            var results = new List<string>();
            foreach (var modelState in pModelState.Values) results.AddRange(modelState.Errors.Select(error => error.ErrorMessage));
            return results;
        }
    }
}