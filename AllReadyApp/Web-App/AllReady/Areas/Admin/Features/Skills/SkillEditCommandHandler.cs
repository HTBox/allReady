using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Skills
{
    public class SkillEditCommandHandler : IAsyncRequestHandler<SkillEditCommand, int>
    {
        private readonly AllReadyContext _context;

        public SkillEditCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(SkillEditCommand message)
        {
            var msgSkill = message.Skill;

            var skill = await _context.Skills.SingleOrDefaultAsync(s => s.Id == msgSkill.Id) ??
                        _context.Add(new Skill()).Entity;

            skill.OwningOrganizationId = msgSkill.OwningOrganizationId;
            skill.Name = msgSkill.Name;
            skill.ParentSkillId = msgSkill.ParentSkillId;
            skill.Description = msgSkill.Description;

            await _context.SaveChangesAsync();

            return skill.Id;
        }
    }
}