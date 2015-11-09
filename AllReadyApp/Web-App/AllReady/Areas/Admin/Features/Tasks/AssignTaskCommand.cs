using MediatR;
using System.Collections.Generic;


namespace AllReady.Areas.Admin.Features.Tasks
{
    public class AssignTaskCommand : IRequest
    {
        public int TaskId { get; set; }
        public List<string> UserIds { get; set; }
    } 
}
