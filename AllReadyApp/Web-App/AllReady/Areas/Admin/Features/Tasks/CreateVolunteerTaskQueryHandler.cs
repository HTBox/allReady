using System.Linq;
using System.Threading.Tasks;

using AllReady.Areas.Admin.ViewModels.VolunteerTask;
using AllReady.Models;

using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class CreateVolunteerTaskQueryHandler : IAsyncRequestHandler<CreateVolunteerTaskQuery, EditViewModel>
    {
        private readonly AllReadyContext context;

        public CreateVolunteerTaskQueryHandler(AllReadyContext context)
        {
            this.context = context;
        }

        public async Task<EditViewModel> Handle(CreateVolunteerTaskQuery message)
        {
            return await context.Events.AsNoTracking()
                .Include(e => e.Location)
                .Include(e => e.Campaign).ThenInclude(c => c.ManagingOrganization)
                .Select(e => new EditViewModel
                {
                    EventId = e.Id,
                    EventName = e.Name,
                    EventStartDate = e.StartDateTime,
                    EventEndDate = e.EndDateTime,
                    StartDateTime = e.StartDateTime,
                    EndDateTime = e.EndDateTime,
                    CampaignId = e.CampaignId,
                    CampaignName = e.Campaign.Name,
                    OrganizationId = e.Campaign.ManagingOrganizationId,
                    TimeZoneId = e.TimeZoneId
                })
                .SingleAsync(e => e.EventId == message.EventId);
        }
    }
}
