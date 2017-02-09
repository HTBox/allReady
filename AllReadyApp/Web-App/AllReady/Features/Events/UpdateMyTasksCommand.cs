using System.Collections.Generic;
using AllReady.ViewModels.Event;
using MediatR;

namespace AllReady.Features.Events
{
    public class UpdateMyTasksCommand : IAsyncRequest
    {
        public string UserId { get; set; }
        public IEnumerable<TaskSignupViewModel> VolunteerTaskSignups { get; set; }
    }
}