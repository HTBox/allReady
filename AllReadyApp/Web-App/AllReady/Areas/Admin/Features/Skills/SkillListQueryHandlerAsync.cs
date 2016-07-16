using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
            List<Skill> skills = new List<Skill>();
            if(message.OrganizationId != null)
            {
                skills = await _context.Skills.AsNoTracking()
                    .Include(s => s.ParentSkill)
                    .Include(s => s.OwningOrganization)
                    .Where(s=>s.OwningOrganizationId == message.OrganizationId)
                    .ToListAsync();
            }
            else
            {
                skills = await _context.Skills.AsNoTracking()
                    .Include(s => s.ParentSkill)
                    .Include(s => s.OwningOrganization)
                    .ToListAsync();
            }          

            List<SkillSummaryModel> results = new List<SkillSummaryModel>();
            foreach(var skill in skills)
            {
                results.Add(new SkillSummaryModel
                {
                    Id = skill.Id,
                    HierarchicalName = skill.HierarchicalName,
                    Description = skill.Description,
                    OwningOrganizationName = skill.OwningOrganization?.Name ?? string.Empty
                });
            }
            return results;
        }
    }
}