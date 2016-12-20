using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AllReady.Areas.Admin.ViewModels.Import
{
    public class IndexViewModel
    {
        public IFormFile File { get; set; }

        public int EventId { get; set; }
        public List<SelectListItem> Events { get; set; }

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