using System;
using System.Collections.Generic;
using MediatR;

namespace AllReady.Hangfire.MediatR
{
    public class SetRequestsToUnassignedCommand : IRequest
    {
        public List<Guid> RequestIds { get; set; }
    }
}
