using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Campaign;
using MediatR;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class PublishViewModelQuery: IAsyncRequest<PublishViewModel>
    {
        public int CampaignId { get; set; }
    }
}
