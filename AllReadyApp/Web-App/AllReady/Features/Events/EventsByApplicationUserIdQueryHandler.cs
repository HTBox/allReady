using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Features.Events
{
    public class EventsByApplicationUserIdQueryHandler : IAsyncRequestHandler<EventsByApplicationUserIdQuery, List<Event>>
    {
        private readonly AllReadyContext _context;

        public EventsByApplicationUserIdQueryHandler(AllReadyContext context)
        {
            _context = context;
        }
        public async Task<List<Event>> Handle(EventsByApplicationUserIdQuery message)
        {
            return await _context.Events.Include(x => x.Organizer)
                    .Where(x => x.Organizer.Id == message.ApplicationUserId)
                    .ToListAsync();
        }
    }
}
