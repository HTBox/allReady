using AllReady.Models;
using MediatR;
using System.Linq;

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
            var campaign = 
                _context.Campaigns.SingleOrDefault(c => c.Id == message.Campaign.Id);

            if (campaign == null)
            {
                campaign = new Campaign();
            }

            campaign.Name = message.Campaign.Name;
            campaign.Description = message.Campaign.Description;
            campaign.StartDateTimeUtc = message.Campaign.StartDate;
            campaign.EndDateTimeUtc = message.Campaign.EndDate;
            campaign.ManagingTenantId = message.Campaign.TenantId;

            _context.Update(campaign);
            _context.SaveChanges();
            return campaign.Id;
        }
    }
}
