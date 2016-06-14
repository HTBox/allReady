using System.Collections.Generic;
using AllReady.ViewModels;
using AllReady.ViewModels.Event;
using MediatR;

namespace AllReady.Features.Event
{
    public class GetMyTasksQuery : IRequest<IEnumerable<TaskSignupViewModel>>
    {
        public int EventId { get; set; }
        public string UserId { get; set; }
    }
}
