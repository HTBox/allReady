using AllReady.Areas.Admin.Models;
using Microsoft.Data.Entity;
using AllReady.Models;
using MediatR;
using System.Linq;
using System.Collections.Generic;
using System;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class EditCampaignCommandHandler : IRequestHandler<EditCampaignCommand, int>
    {
        private AllReadyContext _context;

        public EditCampaignCommandHandler(AllReadyContext context)
        {
            _context = context;

        }
        public int Handle(EditCampaignCommand message)
        {
            var campaign = _context.Campaigns
                                    .Include(l => l.Location).ThenInclude(p => p.PostalCode)
                    .Include(tc => tc.CampaignContacts)
                    .Include(i => i.CampaignImpact)

                .SingleOrDefault(c => c.Id == message.Campaign.Id);

            if (campaign == null)
            {
                campaign = new Campaign();
            }

            campaign.Name = message.Campaign.Name;
            campaign.Description = message.Campaign.Description;
            campaign.TimeZoneId = message.Campaign.TimeZoneId;
            TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(campaign.TimeZoneId);
            var startDateTimeOffset = timeZoneInfo.GetUtcOffset(message.Campaign.StartDate);
            campaign.StartDateTime = new DateTimeOffset(message.Campaign.StartDate.Year, message.Campaign.StartDate.Month, message.Campaign.StartDate.Day, 0, 0, 0, startDateTimeOffset);
            var endDateTimeOffset = timeZoneInfo.GetUtcOffset(message.Campaign.EndDate);
            campaign.EndDateTime = new DateTimeOffset(message.Campaign.EndDate.Year, message.Campaign.EndDate.Month, message.Campaign.EndDate.Day, 23, 59, 59, endDateTimeOffset);
            campaign.ManagingTenantId = message.Campaign.TenantId;
            campaign.ImageUrl = message.Campaign.ImageUrl;

            campaign = campaign.UpdateCampaignContact(message.Campaign, _context);
            campaign.CampaignImpact = campaign.CampaignImpact.UpdateModel(message.Campaign.CampaignImpact);
            campaign.Location = campaign.Location.UpdateModel(message.Campaign.Location);
            if (campaign.CampaignImpact != null)
            {
                _context.Update(campaign.CampaignImpact);
            }
            if (campaign.Location != null)
            {
                _context.Update(campaign.Location);
            }
            _context.Update(campaign);
            _context.SaveChanges();
            return campaign.Id;
        }


    }
}
