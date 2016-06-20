using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationIdByItineraryQueryHandlerAsync : IAsyncRequestHandler<OrganizationIdByIntineraryIdQuery, int>
    {
        private readonly AllReadyContext _context;

        public OrganizationIdByItineraryQueryHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(OrganizationIdByIntineraryIdQuery message)
        {
            return await _context.Itineraries.AsNoTracking()
                .Include(i => i.Event.Campaign)
                .Where(i => i.Id == message.ItineraryId)
                .Select(i => i.Event.Campaign.ManagingOrganizationId)
                .FirstOrDefaultAsync();
        }
    }
}
