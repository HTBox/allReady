using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.CampaignManagerInvites
{
    public class AcceptCampaignManagerInviteCommand : IAsyncRequest
    {
        public int CampaignManagerInviteId { get; set; }
    }
}
