using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationSelectListQueryHandlerAsync : IAsyncRequestHandler<OrganizationSelectListQueryAsync, IEnumerable<SelectListItem>>
    {
        private readonly AllReadyContext _context;
        public OrganizationSelectListQueryHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SelectListItem>> Handle(OrganizationSelectListQueryAsync message)
        {
            return await _context.Organizations.Select(s => new SelectListItem
            {
                    Text = s.Name,
                    Value = s.Id.ToString()
            })
            .ToListAsync();
        }
    }
}