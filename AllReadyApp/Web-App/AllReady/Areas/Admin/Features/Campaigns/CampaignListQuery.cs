using AllReady.Areas.Admin.Models;
using AllReady.ViewModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class CampaignListQuery : IRequest<IEnumerable<CampaignSummaryModel>>
    {
        //public int TenantId { get; set; }
    }
}
