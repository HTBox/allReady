using System;
using AllReady.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Requests
{
    public class ChangeRequestStatusCommand : IAsyncRequest<bool>
    {
        public Guid RequestId { get; set; }
        public RequestStatus NewStatus { get; set; }
    }
}
