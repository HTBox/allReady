using System;
using System.Linq;
using AllReady.Models;
using AllReady.ViewModels;
using MediatR;

namespace AllReady.Features.Campaigns
{
    public class CampaignQueryHandler : IRequestHandler<CampaignQuery, CampaignModel>
    {
        private readonly IAllReadyDataAccess _dataAccess;

        public CampaignQueryHandler(IAllReadyDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public CampaignModel Handle(CampaignQuery message)
        {
            var model = new CampaignModel();
            var results = _dataAccess.Campaigns.Where(c => c.EndDateTime.UtcDateTime.Date > DateTime.UtcNow.Date && !c.Locked).ToViewModel().OrderBy(vm => vm.EndDate).ToList();
            model.CampaignViewModels = results;
            return model;
        }
    }
}
