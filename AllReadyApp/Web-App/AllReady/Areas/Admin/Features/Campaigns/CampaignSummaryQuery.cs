using AllReady.Areas.Admin.Models;
using AllReady.ViewModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class CampaignSummaryQuery : IRequest<CampaignSummaryModel>
    {
        public int CampaignId { get; set; }
    }
}
