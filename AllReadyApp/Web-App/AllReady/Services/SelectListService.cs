using AllReady.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;

namespace AllReady.Services
{
    public class SelectListService : ISelectListService
    {
        private AllReadyContext _context;

        public SelectListService(AllReadyContext context)
        {
            _context = context;
        }

        public IEnumerable<SelectListItem> GetTenants()
        {
            return _context.Tenants.Select(t => new SelectListItem() {Value = t.Id.ToString(), Text = t.Name });
        }

        public IEnumerable<Skill> GetSkills()
        {
            return _context.Skills.ToList()
                //Project HierarchicalName onto Name
                .Select(s => new Skill() { Id = s.Id, Name = s.HierarchicalName, Description = s.Description })
                .OrderBy(s => s.Name);
        }

        public IEnumerable<CampaignImpactType> GetCampaignImpactTypes()
        {
            return _context.CampaignImpactTypes.ToList();
        }
    }
}
