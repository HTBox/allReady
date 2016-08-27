using System.Collections.Generic;
using System.Linq;
using AllReady.Models;
using AllReady.ViewModels.Campaign;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Features.Campaigns
{
    public class UnlockedCampaignsQueryHandler : IRequestHandler<UnlockedCampaignsQuery, List<CampaignViewModel>>
    {
        private readonly AllReadyContext _context;

        public UnlockedCampaignsQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public List<CampaignViewModel> Handle(UnlockedCampaignsQuery message)
        {
            return _context.Campaigns
                   .Include(x => x.ManagingOrganization)
                   .Include(x => x.Events)
                   .Include(x => x.ParticipatingOrganizations)
                   .Where(c => !c.Locked)
                   .ToViewModel()
                   .ToList();
        }
    }
}
