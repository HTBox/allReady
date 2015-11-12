using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Tenants
{
    public class TenantListQueryHandler : IRequestHandler<TenantListQuery, IEnumerable<TenantSummaryModel>>
    {
        private AllReadyContext _context;
        public TenantListQueryHandler(AllReadyContext context)
        {
            _context = context;
        }
        public IEnumerable<TenantSummaryModel> Handle(TenantListQuery message)
        {
            return _context.Tenants.Select(t => new TenantSummaryModel {
                Id = t.Id,
                LogoUrl = t.LogoUrl,
                Name = t.Name,
                WebUrl = t.WebUrl
            });
        }
    }
}
