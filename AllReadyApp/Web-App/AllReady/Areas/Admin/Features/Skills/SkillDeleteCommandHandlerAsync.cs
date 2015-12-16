using AllReady.Models;
using MediatR;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Skills
{
    public class SkillDeleteCommandHandlerAsync : AsyncRequestHandler<SkillDeleteCommandAsync>
    {
        private AllReadyContext _context;
        public SkillDeleteCommandHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        protected override async Task HandleCore(SkillDeleteCommandAsync message)
        {
            var skill = _context.Skills.SingleOrDefault(s => s.Id == message.Id);
            if (skill != null)
            {
                _context.Skills.Remove(skill);
                await _context.SaveChangesAsync();
            }
        }
    }
}