using System.Collections.Generic;
using System.Linq;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationListQueryHandler : IRequestHandler<OrganizationListQuery, IEnumerable<OrganizationSummaryModel>>
    {
        private AllReadyContext _context;
        public OrganizationListQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public IEnumerable<OrganizationSummaryModel> Handle(OrganizationListQuery message)
        {
            return _context.Organizations.Select(t => new OrganizationSummaryModel {
                Id = t.Id,
                LogoUrl = t.LogoUrl,
                Name = t.Name,
                WebUrl = t.WebUrl
            });
        }
    }
}
