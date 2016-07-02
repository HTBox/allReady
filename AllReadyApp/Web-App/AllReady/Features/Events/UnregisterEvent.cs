using System;
using MediatR;

namespace AllReady.Features.Event
{
    [Obsolete("", true)]
    public class UnregisterEvent : IAsyncRequest
    {
        public int EventSignupId { get; set; }
        public string UserId { get; set; }
    }
}
