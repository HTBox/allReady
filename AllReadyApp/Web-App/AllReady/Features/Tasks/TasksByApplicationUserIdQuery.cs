using System.Collections.Generic;
using MediatR;

namespace AllReady.Features.Tasks
{
    public class TasksByApplicationUserIdQuery : IAsyncRequest<List<Models.AllReadyTask>>
    {
        public string ApplicationUserId { get; set; }
    }
}
