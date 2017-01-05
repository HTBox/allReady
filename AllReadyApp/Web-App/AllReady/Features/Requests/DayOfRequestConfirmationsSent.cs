using System;
using System.Collections.Generic;
using MediatR;

namespace AllReady.Features.Requests
{
    public class DayOfRequestConfirmationsSent : INotification
    {
        public List<Guid> RequestIds { get; set; }
    }
}
