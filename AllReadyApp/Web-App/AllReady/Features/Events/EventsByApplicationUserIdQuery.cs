using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace AllReady.Features.Events
{
    public class EventsByApplicationUserIdQuery : IAsyncRequest<List<Models.Event>>
    {
        public string ApplicationUserId { get; set; }
    }
}
