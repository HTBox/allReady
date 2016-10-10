using System;
using System.Collections.Generic;
using MediatR;

namespace AllReady.Areas.Admin.Features.Notifications
{
    public class RequestsAdded : IAsyncNotification
    {
        public List<Guid> RequestIds { get; set; }
    }
}
