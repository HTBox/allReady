using MediatR;
using System.Collections.Generic;


namespace AllReady.Areas.Admin.Features.Tasks
{
    public class AssignTaskCommand : IAsyncRequest
    {
        public int TaskId { get; set; }
        public List<string> UserIds { get; set; }
    } 
}
