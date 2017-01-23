using System.Collections.Generic;
using MediatR;

namespace AllReady.Features.Tasks
{
    public class TasksByApplicationUserIdQuery : IAsyncRequest<List<Models.VolunteerTask>>
    {
        public string ApplicationUserId { get; set; }
    }
}
