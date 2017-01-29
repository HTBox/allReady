using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using AllReady.ViewModels.Event;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Features.Events
{
    public class EventByDateRangeQueryHandler : IAsyncRequestHandler<EventByDateRangeQuery, IEnumerable<EventViewModel>>
    {
        private AllReadyContext _context;

        public EventByDateRangeQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EventViewModel>> Handle(EventByDateRangeQuery message)
        {
            var start = message.StartDate;
            var end = message.EndDate;

            return await _context.Events.AsNoTracking()
                .Where(e => e.StartDateTime <= end && e.EndDateTime >= start)
                .Select(e => new EventViewModel(e))
                .ToListAsync();
        }
    }
}