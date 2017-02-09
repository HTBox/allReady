using System.Collections.Generic;
using MediatR;

namespace AllReady.Features.Notifications
{
    public class TaskAssignedToVolunteersNotification : IAsyncNotification
    {
        public int VolunteerTaskId { get; set; }
        public List<string> NewlyAssignedVolunteers { get; set; }
    }
}