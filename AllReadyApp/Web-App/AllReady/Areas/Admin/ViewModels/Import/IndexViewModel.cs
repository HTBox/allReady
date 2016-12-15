using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AllReady.Areas.Admin.ViewModels.Import
{
    public class IndexViewModel
    {
        public List<ValidationResult> ImportErrors { get; set; } =  new List<ValidationResult>();
        public IFormFile File { get; set; }
        public bool ImportSuccess { get; set; }
    }
}
