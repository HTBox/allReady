using System;
using System.Collections.Generic;
using MediatR;

namespace AllReady.Hangfire.MediatR
{
    public class SetRequstsToUnassignedCommand : IRequest
    {
        public List<Guid> RequestIds { get; set; }
    }
}
