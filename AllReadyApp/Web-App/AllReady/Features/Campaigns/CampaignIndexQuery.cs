using System.Collections.Generic;
using AllReady.Models;
using AllReady.ViewModels;
using MediatR;

namespace AllReady.Features.Campaigns
{
    public class CampaignIndexQuery : IRequest<List<CampaignViewModel>>
    {
    }
}
