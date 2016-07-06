using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationListQueryHandlerAsync : IAsyncRequestHandler<OrganizationListQuery, List<OrganizationSummaryModel>>
    {
        private AllReadyContext _context;
        public OrganizationListQueryHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<List<OrganizationSummaryModel>> Handle(OrganizationListQuery message)
        {
            return await _context.Organizations.Select(t => new OrganizationSummaryModel
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
