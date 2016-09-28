using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Task;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class CreateTaskQueryHandler : IAsyncRequestHandler<CreateTaskQuery, EditViewModel>
    {
        private readonly AllReadyContext context;

        public CreateTaskQueryHandler(AllReadyContext context)
        {
            this.context = context;
        }

        public async Task<EditViewModel> Handle(CreateTaskQuery message)
        {
            return await context.Events.AsNoTracking()
                .Include(e => e.Location)
                .Include(e => e.Campaign).ThenInclude(c => c.ManagingOrganization)
                .Select(e => new EditViewModel
                {
                    EventId = e.Id,
                    EventName = e.Name,
                    StartDateTime = e.StartDateTime,
                    EndDateTime = e.EndDateTime,
                    EventStartDateTime = e.Campaign.StartDateTime,
                    EventEndDateTime = e.Campaign.EndDateTime,
                    CampaignId = e.CampaignId,
                    CampaignName = e.Campaign.Name,
                    OrganizationId = e.Campaign.ManagingOrganizationId,
                    TimeZoneId = e.Campaign.TimeZoneId
                })
                .SingleAsync(e => e.EventId == message.EventId)
                .ConfigureAwait(false);
        }
    }
}