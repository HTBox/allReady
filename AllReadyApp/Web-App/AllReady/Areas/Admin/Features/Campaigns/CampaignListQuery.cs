using AllReady.Areas.Admin.Models;
using MediatR;
using System.Collections.Generic;
 
namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class CampaignListQuery : IRequest<IEnumerable<CampaignSummaryModel>>
    {
         
    }
}
