using System;
using System.Collections.Generic;
using System.Linq;
using AllReady.Models;
using AllReady.ViewModels;
using AllReady.ViewModels.Campaign;
using MediatR;

namespace AllReady.Features.Campaigns
{
    public class CampaignQueryHandler : IRequestHandler<CampaignQuery, List<CampaignViewModel>>
    {
        private readonly IAllReadyDataAccess _dataAccess;

        public CampaignQueryHandler(IAllReadyDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public List<CampaignViewModel> Handle(CampaignQuery message)
        {
            return _dataAccess.Campaigns
                .Where(c => c.EndDateTime.UtcDateTime.Date > DateTime.UtcNow.Date && !c.Locked)
                .ToViewModel()
                .OrderBy(vm => vm.EndDate)
                .ToList();
        }
    }
}
