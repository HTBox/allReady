using System.Collections.Generic;
using MediatR;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class AssignTaskCommandAsync : IAsyncRequest
    {
        public int TaskId { get; set; }
        public List<string> UserIds { get; set; }
    } 
}
