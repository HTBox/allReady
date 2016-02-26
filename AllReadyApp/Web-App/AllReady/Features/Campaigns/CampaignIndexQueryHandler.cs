using System.Collections.Generic;
using System.Linq;
using AllReady.Models;
using AllReady.ViewModels;
using MediatR;

namespace AllReady.Features.Campaigns
{
    public class CampaignIndexQueryHandler : IRequestHandler<CampaignIndexQuery, List<CampaignViewModel>>
    {
        private readonly IAllReadyDataAccess _dataAccess;

        public CampaignIndexQueryHandler(IAllReadyDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public List<CampaignViewModel> Handle(CampaignIndexQuery message)
        {
            return _dataAccess.Campaigns.Where(c => !c.Locked).ToViewModel().ToList();
        }
    }
}
