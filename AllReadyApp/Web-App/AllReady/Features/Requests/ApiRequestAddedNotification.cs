using System;
using MediatR;

namespace AllReady.Features.Requests
{
    public class ApiRequestAddedNotification : IAsyncNotification
    {
        public Guid RequestId { get; set; }
    }
}
