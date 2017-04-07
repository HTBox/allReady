using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Events
{
    public class DeleteQueryHandler : IAsyncRequestHandler<DeleteQuery, DeleteViewModel>
    {
        private readonly AllReadyContext _context;

        public DeleteQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<DeleteViewModel> Handle(DeleteQuery message)
        {
            return await _context.Events.AsNoTracking()
                .Include(e => e.Campaign)
                .ThenInclude(c => c.ManagingOrganization)
                .Select(@event => new DeleteViewModel
                {
                    Id = @event.Id,
                    Name = @event.Name,
                    CampaignId = @event.Campaign.Id,
                    CampaignName = @event.Campaign.Name,
                    OrganizationId = @event.Campaign.ManagingOrganization.Id,
                    StartDateTime = @event.StartDateTime,
                    EndDateTime = @event.EndDateTime
                })
                .SingleAsync(t => t.Id == message.EventId);
        }
    }
}
