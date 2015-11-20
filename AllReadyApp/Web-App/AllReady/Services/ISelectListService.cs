using AllReady.Models;
using Microsoft.AspNet.Mvc.Rendering;
using System.Collections.Generic;

namespace AllReady.Services
{
    public interface ISelectListService
    {
        IEnumerable<SelectListItem> GetTenants();
        IEnumerable<Skill> GetSkills();
        IEnumerable<SelectListItem> GetCampaignImpactTypes();
    }
}
