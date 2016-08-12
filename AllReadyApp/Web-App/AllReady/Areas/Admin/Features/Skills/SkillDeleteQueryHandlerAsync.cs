using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Skill;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Skills
{
    public class SkillDeleteQueryHandlerAsync : IAsyncRequestHandler<SkillDeleteQueryAsync, SkillDeleteModel>
    {
        private AllReadyContext _context;
        public SkillDeleteQueryHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }
        public async Task<SkillDeleteModel> Handle(SkillDeleteQueryAsync message)
        {
            var skill = await _context.Skills.AsNoTracking()
                .Include(s => s.ParentSkill)
                .Include(s => s.OwningOrganization)
                .SingleOrDefaultAsync(s => s.Id == message.Id);

            if (skill == null) return null;

            var model = new SkillDeleteModel
            {
                HierarchicalName = skill.HierarchicalName,
                OwningOrganizationId = skill.OwningOrganizationId,
            };

            var children = await _context.Skills
                .Where(s => s.ParentSkillId == message.Id)
                .Select(s => s.HierarchicalName)
                .ToArrayAsync();

            if(children != null)
            {
                model.ChildrenNames = children;
            }

            return model;
        }
    }
}