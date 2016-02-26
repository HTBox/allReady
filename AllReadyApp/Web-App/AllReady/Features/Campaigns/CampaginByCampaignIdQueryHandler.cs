using AllReady.Models;
using MediatR;

namespace AllReady.Features.Campaigns
{
    public class CampaginByCampaignIdQueryHandler : IRequestHandler<CampaginByCampaignIdQuery, Campaign>
    {
        private readonly IAllReadyDataAccess _dataAccess;

        public CampaginByCampaignIdQueryHandler(IAllReadyDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public Campaign Handle(CampaginByCampaignIdQuery message)
        {
            return _dataAccess.GetCampaign(message.CampaignId);
        }
    }
}
