using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;

namespace AllReady.Extensions
{
    public static class ModelStateExtensions
    {
        public static List<string> GetErrorMessages(this ModelStateDictionary modelStateDictionary)
        {
            var results = new List<string>();
            foreach (var modelState in modelStateDictionary.Values) results.AddRange(modelState.Errors.Select(error => error.ErrorMessage));
            return results;
        }

        public static ModelErrorCollection GetErrorMessagesByKey(this ModelStateDictionary modelStateDictionary, string key)
        {
            ModelStateEntry value;
            var success = modelStateDictionary.TryGetValue(key, out value);
            if (success)
                return value.Errors;

            return new ModelErrorCollection();
        }
    }
}