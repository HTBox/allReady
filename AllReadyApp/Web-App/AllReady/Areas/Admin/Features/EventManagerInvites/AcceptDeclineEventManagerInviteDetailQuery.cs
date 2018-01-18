using AllReady.Areas.Admin.ViewModels.ManagerInvite;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.EventManagerInvites
{
    public class AcceptDeclineEventManagerInviteDetailQuery : IAsyncRequest<AcceptDeclineEventManagerInviteViewModel>
    {
        public int EventManagerInviteId { get; set; }
    }
}
