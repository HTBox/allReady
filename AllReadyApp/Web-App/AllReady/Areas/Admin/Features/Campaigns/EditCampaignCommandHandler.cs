using AllReady.Areas.Admin.Models;
using Microsoft.Data.Entity;
using AllReady.Models;
using MediatR;
using System.Linq;
using System.Collections.Generic;

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

                .SingleOrDefault(c => c.Id == message.Campaign.Id);

            if (campaign == null)
            {
                campaign = new Campaign();
            }

            campaign.Name = message.Campaign.Name;
            campaign.Description = message.Campaign.Description;
            campaign.StartDateTimeUtc = message.Campaign.StartDate;
            campaign.EndDateTimeUtc = message.Campaign.EndDate;
            campaign.ManagingTenantId = message.Campaign.TenantId;

            campaign = campaign.UpdateCampaignContact(message.Campaign, _context);
            campaign.Location = campaign.Location.UpdateModel(message.Campaign.Location);
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
