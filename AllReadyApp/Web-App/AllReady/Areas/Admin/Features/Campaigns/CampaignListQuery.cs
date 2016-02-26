using System.Collections.Generic;
using AllReady.Areas.Admin.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class CampaignListQuery : IRequest<IEnumerable<CampaignSummaryModel>>
    {
         public int? OrganizationId { get; set; }
    }
}
