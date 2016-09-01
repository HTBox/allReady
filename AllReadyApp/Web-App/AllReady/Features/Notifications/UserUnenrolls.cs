using MediatR;

namespace AllReady.Features.Notifications
{
    public class UserUnenrolls : IAsyncNotification
    {
        public string UserId { get; set; }
        public int TaskId { get; set; }
    }
}
