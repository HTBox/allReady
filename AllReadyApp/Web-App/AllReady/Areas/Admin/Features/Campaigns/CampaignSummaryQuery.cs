using AllReady.Areas.Admin.ViewModels;
using AllReady.ViewModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class CampaignSummaryQuery : IRequest<CampaignSummaryViewModel>
    {
        public int CampaignId { get; set; }
    }
}
