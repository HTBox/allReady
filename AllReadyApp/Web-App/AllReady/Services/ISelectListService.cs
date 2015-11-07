using Microsoft.AspNet.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Services
{
    public interface ISelectListService
    {
        IEnumerable<SelectListItem> GetTenants();
        IEnumerable<SelectListItem> GetSkills();
    }
}
