using System.Threading.Tasks;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Skills
{
    public class SkillEditQueryHandlerAsync : IAsyncRequestHandler<SkillEditQueryAsync, SkillEditModel>
    {
        private AllReadyContext _context;
        public SkillEditQueryHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }
        public async Task<SkillEditModel> Handle(SkillEditQueryAsync message)
        {
            var skill = await _context.Skills.AsNoTracking()
                .Include(s => s.ParentSkill)
                .Include(s => s.OwningOrganization)
                .SingleOrDefaultAsync(s => s.Id == message.Id);

            if (skill == null) return null;

            return new SkillEditModel
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