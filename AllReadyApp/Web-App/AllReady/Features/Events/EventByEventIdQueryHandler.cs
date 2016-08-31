using System.Linq;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Features.Event
{
    public class EventByIdQueryHandler : IRequestHandler<EventByIdQuery, Models.Event>
    {
        private readonly AllReadyContext dataContext;

        public EventByIdQueryHandler(AllReadyContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public Models.Event Handle(EventByIdQuery message)
        {
            // TODO: can we leave off some of these .Include()?
            return this.dataContext.Events
                .Include(a => a.Location)
                .Include(a => a.Campaign).ThenInclude(c => c.ManagingOrganization)
                .Include(a => a.RequiredSkills).ThenInclude(rs => rs.Skill).ThenInclude(s => s.ParentSkill)
                .Include(a => a.Tasks).ThenInclude(t => t.AssignedVolunteers).ThenInclude(tu => tu.User)
                .Include(a => a.Tasks).ThenInclude(t => t.RequiredSkills).ThenInclude(ts => ts.Skill)
                .Include(a => a.UsersSignedUp).ThenInclude(u => u.User)
                .SingleOrDefault(a => a.Id == message.EventId);
        }
    }
}
