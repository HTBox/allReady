using System;
using System.Collections.Generic;
using MediatR;

namespace AllReady.Areas.Admin.Features.Requests
{
    public class SetRequstsToUnassignedCommand : IRequest
    {
        public List<Guid> RequestIds { get; set; }
    }
}
