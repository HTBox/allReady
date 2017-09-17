using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Features.Events
{
    public class EventByEventIdQueryHandler : IAsyncRequestHandler<EventByEventIdQuery, Event>
    {
        private readonly AllReadyContext _context;

        public EventByEventIdQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<Event> Handle(EventByEventIdQuery message)
        {
            // TODO: can we leave off some of these .Include()?
            return await _context.Events
                .Include(a => a.Location)
                .Include(a => a.Campaign).ThenInclude(c => c.ManagingOrganization)
                .Include(a => a.RequiredSkills).ThenInclude(rs => rs.Skill).ThenInclude(s => s.ParentSkill)
                .Include(a => a.VolunteerTasks).ThenInclude(t => t.AssignedVolunteers).ThenInclude(tu => tu.User)
                .Include(a => a.VolunteerTasks).ThenInclude(t => t.RequiredSkills).ThenInclude(ts => ts.Skill)
                .SingleOrDefaultAsync(a => a.Id == message.EventId);
        }
    }
}
