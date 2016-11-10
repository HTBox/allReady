using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Skill;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Skills
{
    public class SkillListQueryHandler : IAsyncRequestHandler<SkillListQuery, IEnumerable<SkillSummaryViewModel>>
    {
        private readonly AllReadyContext _context;

        public SkillListQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SkillSummaryViewModel>> Handle(SkillListQuery message)
        {
            List<Skill> skills; // assigned below

            if (message.OrganizationId != null)
            {
                skills = await _context.Skills.AsNoTracking()
                    .Include(s => s.ParentSkill)
                    .Include(s => s.ChildSkills)
                    .Include(s => s.OwningOrganization)
                    .Where(s => s.OwningOrganizationId == message.OrganizationId)
                    .Where(s => s.HierarchicalName != Skill.InvalidHierarchy)
                    .ToListAsync();
            }
            else
            {
                skills = await _context.Skills.AsNoTracking()
                    .Include(s => s.ParentSkill)
                    .Include(s => s.ChildSkills)
                    .Include(s => s.OwningOrganization)
                    .Where(s => s.HierarchicalName != Skill.InvalidHierarchy)
                    .ToListAsync();
            }

            return skills.Select(skill => new SkillSummaryViewModel
            {
                Id = skill.Id,
                HierarchicalName = skill.HierarchicalName,
                Description = skill.Description,
                OwningOrganizationName = skill.OwningOrganization?.Name ?? string.Empty,
                DescendantIds = skill.DescendantIds
            }).ToList();
        }
    }
}