using System.Collections.Generic;
using MediatR;

namespace AllReady.Features.Notifications
{
    public class TaskAssignedToVolunteersNotification : IAsyncNotification
    {
        public int TaskId { get; set; }
        public List<string> NewlyAssignedVolunteers { get; set; }
    }
}