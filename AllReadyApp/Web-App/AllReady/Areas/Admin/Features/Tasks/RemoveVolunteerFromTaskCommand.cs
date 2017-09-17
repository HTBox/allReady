using MediatR;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class RemoveVolunteerFromTaskCommand : IAsyncRequest
    {
        /// <summary>
        ///     If true, inform the user, through email, that it has been removed.
        /// </summary>
        public bool NotifyUser { get; set; }

        /// <summary>
        ///     The volunteer task that the user should be removed from
        /// </summary>
        public int TaskId { get; set; }

        /// <summary>
        ///     User to remove
        /// </summary>
        public string UserId { get; set; }
    }
}
