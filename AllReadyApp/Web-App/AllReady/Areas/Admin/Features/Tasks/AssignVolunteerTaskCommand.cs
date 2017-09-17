using System.Collections.Generic;
using MediatR;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class AssignVolunteerTaskCommand : IAsyncRequest
    {
        public int VolunteerTaskId { get; set; }
        public List<string> UserIds { get; set; }
    }
}
