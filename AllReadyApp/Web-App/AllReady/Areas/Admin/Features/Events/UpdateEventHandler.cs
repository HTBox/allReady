using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using System.Linq;

namespace AllReady.Areas.Admin.Features.Events
{
    public class UpdateEventHandler : AsyncRequestHandler<UpdateEvent>
    {
        private readonly AllReadyContext _context;

        public UpdateEventHandler(AllReadyContext context)
        {
            _context = context;
        }

        protected override async Task HandleCore(UpdateEvent message)
        {
            //First remove any skills that are no longer associated with this event
            var eventSkillsToRemove = _context.EventSkills.Where(es => es.EventId == message.Event.Id && (message.Event.RequiredSkills == null || 
                !message.Event.RequiredSkills.Any(es1 => es1.SkillId == es.SkillId)));
            _context.EventSkills.RemoveRange(eventSkillsToRemove);

            _context.Events.Update(message.Event);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}