﻿using AllReady.Models;
using MediatR;
using System.Linq;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class DeleteCampaignCommandHandler : RequestHandler<DeleteCampaignCommand>
    {
        private IAllReadyContext _context;

        public DeleteCampaignCommandHandler(AllReadyContext context)
        {
            _context = context;

        }
        protected override void HandleCore(DeleteCampaignCommand message)
        {
            var campaign = 
                _context.Campaigns.SingleOrDefault(c => c.Id == message.CampaignId);

            if (campaign != null)
            {
                _context.Campaigns.Remove(campaign);
                _context.SaveChanges();
            }
        }
    }
}
