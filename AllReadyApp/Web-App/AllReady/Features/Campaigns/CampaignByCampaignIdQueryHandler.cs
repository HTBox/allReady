using AllReady.Models;
using MediatR;

namespace AllReady.Features.Campaigns
{
    public class CampaignByCampaignIdQueryHandler : IRequestHandler<CampaignByCampaignIdQuery, Campaign>
    {
        private readonly IAllReadyDataAccess _dataAccess;

        public CampaignByCampaignIdQueryHandler(IAllReadyDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public Campaign Handle(CampaignByCampaignIdQuery message)
        {
            return _dataAccess.GetCampaign(message.CampaignId);
        }
    }
}
