using System.Collections.Generic;
using AllReady.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace AllReady.Services
{
    public interface ISelectListService
    {
        IEnumerable<SelectListItem> GetOrganizations(ClaimsPrincipal user);
        IEnumerable<Skill> GetSkills();
        IEnumerable<SelectListItem> GetCampaignImpactTypes();
        IEnumerable<SelectListItem> GetTimeZones();
    }
}
