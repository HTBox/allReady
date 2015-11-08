using AllReady.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.ViewModels
{
    public class SiteAdminViewModel
    {
        public List<ApplicationUser> Users { get; set; }
    }
}
