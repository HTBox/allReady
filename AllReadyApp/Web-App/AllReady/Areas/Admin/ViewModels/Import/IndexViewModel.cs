using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AllReady.Areas.Admin.ViewModels.Import
{
    public class IndexViewModel
    {
        public IFormFile File { get; set; }
        public List<string> ImportErrors { get; set; } = new List<string>();
        public List<ValidationError> ValidationErrors { get; set; } = new List<ValidationError>();
        public bool ImportSuccess { get; set; }

        public class ValidationError
        {
            public string ProviderRequestId { get; set; }
            public List<ValidationResult> Errors { get; set; } = new List<ValidationResult>();
        }
    }
}