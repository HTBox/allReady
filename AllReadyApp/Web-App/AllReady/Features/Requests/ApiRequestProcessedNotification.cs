using System;
using MediatR;

namespace AllReady.Features.Requests
{
    public class ApiRequestProcessedNotification : INotification
    {
        public Guid RequestId { get; set; }
        
        public bool Acceptance { get; set; }
    }
}
