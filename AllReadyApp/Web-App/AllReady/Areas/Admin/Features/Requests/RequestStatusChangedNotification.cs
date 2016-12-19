using System;
using AllReady.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Requests
{
    public class RequestStatusChangedNotification : IAsyncNotification
    {
        public Guid RequestId { get; set; }
        public RequestStatus NewStatus { get; set; }
        public RequestStatus OldStatus { get; set; }
    }
}