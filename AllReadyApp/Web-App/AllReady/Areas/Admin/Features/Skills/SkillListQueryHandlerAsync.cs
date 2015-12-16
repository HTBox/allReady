using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Skills
{
    public class SkillListQueryHandlerAsync : IAsyncRequestHandler<SkillListQueryAsync, IEnumerable<SkillSummaryModel>>
    {
        private AllReadyContext _context;
        public SkillListQueryHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SkillSummaryModel>> Handle(SkillListQueryAsync message)
        {
            return await _context.Skills
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