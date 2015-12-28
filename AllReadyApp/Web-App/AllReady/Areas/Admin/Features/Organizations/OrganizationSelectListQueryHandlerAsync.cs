using AllReady.Models;
using MediatR;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationSelectListQueryHandlerAsync : IAsyncRequestHandler<OrganizationSelectListQueryAsync, IEnumerable<SelectListItem>>
    {
        private AllReadyContext _context;
        public OrganizationSelectListQueryHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SelectListItem>> Handle(OrganizationSelectListQueryAsync message)
        {
            return await _context.Organizations
                .Select(s => new SelectListItem
                {
                     Text = s.Name,
                     Value = s.Id.ToString()
                })
                .ToListAsync();
        }
    }
}