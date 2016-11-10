using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Skill;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Skills
{
    public class SkillEditQueryHandler : IAsyncRequestHandler<SkillEditQuery, SkillEditViewModel>
    {
        private readonly AllReadyContext _context;

        public SkillEditQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<SkillEditViewModel> Handle(SkillEditQuery message)
        {
            var skill = await _context.Skills.AsNoTracking()
                .Include(s => s.ParentSkill)
                .Include(s => s.ChildSkills)
                .Include(s => s.OwningOrganization)
                .SingleOrDefaultAsync(s => s.Id == message.Id);

            if (skill == null) return null;

            return new SkillEditViewModel
            {
                Id = skill.Id,
                Name = skill.Name,
                Description = skill.Description,
                ParentSkillId = skill.ParentSkillId,
                OwningOrganizationId = skill.OwningOrganizationId,
                OwningOrganizationName = skill.OwningOrganization?.Name ?? string.Empty
            };
        }
    }
}