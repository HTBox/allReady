using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace AllReady.Areas.Admin.Features.Goals
{
    public class GoalDeleteCommand : IAsyncRequest
    {
        public int GoalId { get; set; }
    }
}
