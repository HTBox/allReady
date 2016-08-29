using System.Collections.Generic;
using AllReady.Models;

namespace AllReady.Areas.Admin.ViewModels.Site
{
    public class IndexViewModel
    {
        public List<ApplicationUser> Users { get; set; }
    }
}
