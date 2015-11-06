using AllReady.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc.Rendering;

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
            return _context.Tenants.Select(t => new SelectListItem() { Value = t.Id.ToString(), Text = t.Name });
        }

        public IEnumerable<SelectListItem> GetSkills()
        {
            return _context.Skills.ToList()
                .Select(s => new SelectListItem() { Value = s.Id.ToString(), Text = s.HierarchicalName })
                .OrderBy(s => s.Text);
        }
    }
}
