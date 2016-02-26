using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;

namespace AllReady.Areas.Admin.Features.Skills
{
    public class SkillEditCommandHandlerAsync : IAsyncRequestHandler<SkillEditCommandAsync, int>
    {
        private AllReadyContext _context;

        public SkillEditCommandHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(SkillEditCommandAsync message)
        {
            var msgSkill = message.Skill;

            Skill skill = new Skill();

            if(msgSkill.Id > 0)
            {
                skill = await _context.Skills.SingleOrDefaultAsync(s => s.Id == msgSkill.Id);
            }

            skill.OwningOrganizationId = msgSkill.OwningOrganizationId;
            skill.Name = msgSkill.Name;
            skill.ParentSkillId = msgSkill.ParentSkillId;
            skill.Description = msgSkill.Description;

            _context.Skills.Update(skill);
            await _context.SaveChangesAsync();

            return skill.Id;
        }
    }
}