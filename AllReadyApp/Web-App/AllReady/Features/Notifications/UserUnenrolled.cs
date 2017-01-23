using MediatR;

namespace AllReady.Features.Notifications
{
    public class UserUnenrolled : IAsyncNotification
    {
        public string UserId { get; set; }
        public int VolunteerTaskId { get; set; }
    }
}
