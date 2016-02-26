using System.Collections.Generic;
using AllReady.ViewModels;
using MediatR;

namespace AllReady.Features.Campaigns
{
    public class CampaignGetQuery : IRequest<List<CampaignViewModel>>
    {
    }
}
