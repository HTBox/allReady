using System.Collections.Generic;
using System.Linq;
using AllReady.Models;
using AllReady.ViewModels;
using AllReady.ViewModels.Campaign;
using MediatR;

namespace AllReady.Features.Campaigns
{
    public class UnlockedCampaignsQueryHandler : IRequestHandler<UnlockedCampaignsQuery, List<CampaignViewModel>>
    {
        private readonly IAllReadyDataAccess _dataAccess;

        public UnlockedCampaignsQueryHandler(IAllReadyDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
        public List<CampaignViewModel> Handle(UnlockedCampaignsQuery message)
        {
            return _dataAccess.Campaigns.Where(c => !c.Locked).ToViewModel().ToList();
        }
    }
}
