using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Events
{
    public class OrganizationIdByEventIdQueryHandler : IAsyncRequestHandler<OrganizationIdByEventIdQuery, int>
    {
        private readonly AllReadyContext _context;

        public OrganizationIdByEventIdQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(OrganizationIdByEventIdQuery message)
        {
            var @event = await _context.Events.AsNoTracking()
                .Include(e => e.Campaign)
                .ThenInclude(c => c.ManagingOrganization)
                .SingleAsync(t => t.Id == message.EventId)
                .ConfigureAwait(false);

            return @event.Campaign.ManagingOrganization.Id;
        }
    }
}
