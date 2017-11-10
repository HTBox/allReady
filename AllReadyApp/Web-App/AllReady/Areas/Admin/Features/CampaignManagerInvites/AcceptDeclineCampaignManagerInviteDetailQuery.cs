using AllReady.Areas.Admin.ViewModels.ManagerInvite;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.CampaignManagerInvites
{
    public class AcceptDeclineCampaignManagerInviteDetailQuery : IAsyncRequest<AcceptDeclineCampaignManagerInviteViewModel>
    {
        public int CampaignManagerInviteId { get; set; }
    }
}
