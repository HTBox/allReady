using System;
using System.Collections.Generic;
using System.Linq;
using AllReady.Models;
using AllReady.ViewModels.Campaign;
using MediatR;

namespace AllReady.Features.Campaigns
{
    public class CampaignByApplicationUserIdQuery : IAsyncRequest<List<Campaign>>
    {
        public string ApplicationUserId { get; set; }
    }
}
