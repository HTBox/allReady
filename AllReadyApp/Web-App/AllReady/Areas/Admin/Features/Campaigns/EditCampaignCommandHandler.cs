using System;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class EditCampaignCommandHandler : IAsyncRequestHandler<EditCampaignCommand, int>
    {
        private AllReadyContext _context;

        public EditCampaignCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(EditCampaignCommand message)
        {
            var campaign = await _context.Campaigns
                .Include(l => l.Location)
                .Include(tc => tc.CampaignContacts)
                .Include(i => i.CampaignImpact)
                .SingleOrDefaultAsync(c => c.Id == message.Campaign.Id)
                .ConfigureAwait(false);

            if (campaign == null)
            {
                campaign = new Campaign();
            }

            campaign.Name = message.Campaign.Name;
            campaign.Description = message.Campaign.Description;
            campaign.FullDescription = message.Campaign.FullDescription;
            campaign.ExternalUrl = message.Campaign.ExternalUrl;
            campaign.ExternalUrlText = message.Campaign.ExternalUrlText;

            campaign.TimeZoneId = message.Campaign.TimeZoneId;

            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(campaign.TimeZoneId);
            var startDateTimeOffset = timeZoneInfo.GetUtcOffset(message.Campaign.StartDate);
            campaign.StartDateTime = new DateTimeOffset(message.Campaign.StartDate.Year, message.Campaign.StartDate.Month, message.Campaign.StartDate.Day, 0, 0, 0, startDateTimeOffset);

            var endDateTimeOffset = timeZoneInfo.GetUtcOffset(message.Campaign.EndDate);
            campaign.EndDateTime = new DateTimeOffset(message.Campaign.EndDate.Year, message.Campaign.EndDate.Month, message.Campaign.EndDate.Day, 23, 59, 59, endDateTimeOffset);

            campaign.ManagingOrganizationId = message.Campaign.OrganizationId;
            campaign.ImageUrl = message.Campaign.ImageUrl;

            campaign = campaign.UpdateCampaignContact(message.Campaign, _context);
            campaign.CampaignImpact = campaign.CampaignImpact.UpdateModel(message.Campaign.CampaignImpact);
            campaign.Location = campaign.Location.UpdateModel(message.Campaign.Location);

            campaign.Featured = message.Campaign.Featured;

            if (campaign.CampaignImpact != null)
            {
                _context.Update(campaign.CampaignImpact);
            }

            if (campaign.Location != null)
            {
                _context.Update(campaign.Location);
            }

            _context.Update(campaign);

            await _context.SaveChangesAsync().ConfigureAwait(false);

            return campaign.Id;
        }
    }
}
