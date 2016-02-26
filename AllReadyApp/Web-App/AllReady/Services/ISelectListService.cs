using System.Collections.Generic;
using AllReady.Models;
using Microsoft.AspNet.Mvc.Rendering;

namespace AllReady.Services
{
    public interface ISelectListService
    {
        IEnumerable<SelectListItem> GetOrganizations();
        IEnumerable<Skill> GetSkills();
        IEnumerable<SelectListItem> GetCampaignImpactTypes();
        IEnumerable<SelectListItem> GetTimeZones();
    }
}
