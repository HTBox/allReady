using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Skills
{
    public class TenantSkillListQueryHandlerAsync : IAsyncRequestHandler<TenantSkillListQueryAsync, IEnumerable<SkillSummaryModel>>
    {
        private AllReadyContext _context;
        public TenantSkillListQueryHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SkillSummaryModel>> Handle(TenantSkillListQueryAsync message)
        {
            return await _context.Skills
                .Where(s => s.OwningOrganizationId == message.Id)
                .Select(s => new SkillSummaryModel
                {
                    Id = s.Id,
                    HierarchicalName = s.HierarchicalName,
                    Description = s.Description,
                    OwningOrganizationName = s.OwningOrganization.Name
                })
                .ToListAsync();
        }
    }
}