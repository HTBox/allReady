using AllReady.Areas.Admin.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class EditCampaignCommand : IRequest<int>
    {
        public CampaignSummaryModel Campaign {get; set;}
    }
}
