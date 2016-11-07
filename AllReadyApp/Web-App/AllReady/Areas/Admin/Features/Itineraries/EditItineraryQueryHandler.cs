using AllReady.Areas.Admin.ViewModels.Itinerary;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class EditItineraryQueryHandler : IAsyncRequestHandler<EditItineraryQuery, ItineraryEditViewModel>
    {
        private readonly AllReadyContext _context;
        private readonly IMediator _mediator;

        public EditItineraryQueryHandler(AllReadyContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<ItineraryEditViewModel> Handle(EditItineraryQuery message)
        {
            return await _context.Itineraries.AsNoTracking()
                .Include(rec => rec.Event).ThenInclude(rec => rec.Campaign)
                .Select(rec => new ItineraryEditViewModel
                {
                    Id = rec.Id,
                    Name = rec.Name,
                    Date = rec.Date,
                    EventId = rec.EventId,
                    OrganizationId = rec.Event.Campaign.ManagingOrganizationId,
                    EventName = rec.Event.Name,
                    CampaignId = rec.Event.CampaignId,
                    CampaignName = rec.Event.Campaign.Name
                })
                .SingleOrDefaultAsync();
        }
    }
}
