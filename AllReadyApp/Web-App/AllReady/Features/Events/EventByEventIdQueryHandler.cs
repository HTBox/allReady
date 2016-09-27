using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Features.Events
{
    public class EventByEventIdQueryHandlerAsync : IAsyncRequestHandler<EventByEventIdQueryAsync, Event>
    {
        private readonly AllReadyContext _context;

        public EventByEventIdQueryHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<Event> Handle(EventByEventIdQueryAsync message)
        {
            // TODO: can we leave off some of these .Include()?
            return await _context.Events
                .Include(a => a.Location)
                .Include(a => a.Campaign).ThenInclude(c => c.ManagingOrganization)
                .Include(a => a.RequiredSkills).ThenInclude(rs => rs.Skill).ThenInclude(s => s.ParentSkill)
                .Include(a => a.Tasks).ThenInclude(t => t.AssignedVolunteers).ThenInclude(tu => tu.User)
                .Include(a => a.Tasks).ThenInclude(t => t.RequiredSkills).ThenInclude(ts => ts.Skill)
                .SingleOrDefaultAsync(a => a.Id == message.EventId)
                .ConfigureAwait(false);
        }
    }
}
