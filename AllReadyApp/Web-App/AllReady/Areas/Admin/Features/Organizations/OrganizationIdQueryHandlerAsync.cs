using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationIdQueryHandlerAsync : IAsyncRequestHandler<OrganizationIdQuery, int>
    {
        private readonly AllReadyContext _context;

        public OrganizationIdQueryHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(OrganizationIdQuery message)
        {
            return await _context.Itineraries.AsNoTracking()
                .Include(i => i.Event.Campaign)
                .Where(i => i.Id == message.ItineraryId)
                .Select(i => i.Event.Campaign.ManagingOrganizationId)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
        }
    }
}
