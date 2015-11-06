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
            return _context.Tenants.Select(t => new SelectListItem() {Value = t.Id.ToString(), Text = t.Name });
        }
    }
}
