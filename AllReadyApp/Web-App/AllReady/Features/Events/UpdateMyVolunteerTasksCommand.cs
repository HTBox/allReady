using System.Collections.Generic;
using AllReady.ViewModels.Event;
using MediatR;

namespace AllReady.Features.Events
{
    public class UpdateMyVolunteerTasksCommand : IAsyncRequest
    {
        public string UserId { get; set; }
        public IEnumerable<VolunteerTaskSignupViewModel> VolunteerTaskSignups { get; set; }
    }
}