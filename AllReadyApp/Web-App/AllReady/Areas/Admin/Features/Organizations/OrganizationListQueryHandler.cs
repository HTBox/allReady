using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Organization;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationListQueryHandler : IAsyncRequestHandler<OrganizationListQuery, List<OrganizationSummaryViewModel>>
    {
        private AllReadyContext _context;
        public OrganizationListQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<List<OrganizationSummaryViewModel>> Handle(OrganizationListQuery message)
        {
            return await _context.Organizations.Select(t => new OrganizationSummaryViewModel
            {
                Id = t.Id,
                LogoUrl = t.LogoUrl,
                Name = t.Name,
                WebUrl = t.WebUrl
            })
            .ToListAsync()
            .ConfigureAwait(false);
        }
    }
}
