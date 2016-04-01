using System.Collections.Generic;
using AllReady.ViewModels;
using MediatR;

namespace AllReady.Features.Activity
{
    public class GetMyTasksQuery : IRequest<IEnumerable<TaskSignupViewModel>>
    {
        public int ActivityId { get; set; }
        public string UserId { get; set; }
    }
}
