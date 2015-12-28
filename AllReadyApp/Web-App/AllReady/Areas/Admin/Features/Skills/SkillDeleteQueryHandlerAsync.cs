using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

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
                .Include(s => s.Children)
                .SingleOrDefaultAsync(s => s.Id == message.Id);

            if (skill == null) return null;

            return new SkillDeleteModel
            {
                HierarchicalName = skill.HierarchicalName,
                OwningOrganizationId = skill.OwningOrganizationId,
                ChildrenNames = skill.Children.Select(c => c.HierarchicalName).ToList()
            };
        }
    }
}