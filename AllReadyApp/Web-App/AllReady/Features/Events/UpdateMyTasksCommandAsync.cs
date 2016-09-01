using System.Collections.Generic;
using AllReady.ViewModels.Event;
using MediatR;

namespace AllReady.Features.Events
{
    public class UpdateMyTasksCommandAsync : IAsyncRequest
    {
        public string UserId { get; set; }
        public IEnumerable<TaskSignupViewModel> TaskSignups { get; set; }
    }
}