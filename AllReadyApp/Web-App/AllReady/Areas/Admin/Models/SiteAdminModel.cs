using AllReady.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Models
{
    public class SiteAdminModel
    {
        public List<ApplicationUser> Users { get; set; }
    }
}
