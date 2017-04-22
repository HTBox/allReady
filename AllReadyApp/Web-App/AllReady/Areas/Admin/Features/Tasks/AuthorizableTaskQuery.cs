using AllReady.Security;
using MediatR;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Tasks
{
    /// <summary>
    /// Uses an <see cref="IAuthorizableTaskBuilder"/> to build a <see cref="IAuthorizableTask"/>
    /// </summary>
    public class AuthorizableTaskQuery : IAsyncRequest<IAuthorizableTask>
    {
        /// <summary>
        /// Initializes a new instance of an <see cref="AuthorizableTaskQuery"/>.
        /// Uses an <see cref="IAuthorizableTaskBuilder"/> to build a <see cref="IAuthorizableTask"/>
        /// </summary>
        public AuthorizableTaskQuery(int taskId, int? eventId = null, int? campaignId = null, int? orgId = null)
        {
            TaskId = taskId;
            EventId = eventId;
            CampaignId = campaignId;
            OrganizationId = orgId;
        }

        /// <summary>
        /// The task ID
        /// </summary>
        public int TaskId { get; }

        /// <summary>
        /// The event ID for the task, if known
        /// </summary>
        public int? EventId { get; }

        /// <summary>
        /// The organization ID for the task, if known
        /// </summary>
        public int? OrganizationId { get; }

        /// <summary>
        /// The campaign ID for the task, if known
        /// </summary>
        public int? CampaignId { get; }
    }

    /// <summary>
    /// Handles an <see cref="AuthorizableTaskQuery"/>
    /// </summary>
    public class AuthorizableTaskQueryHandler : IAsyncRequestHandler<AuthorizableTaskQuery, IAuthorizableTask>
    {
        private readonly IAuthorizableTaskBuilder _authorizableTaskBuilder;

        public AuthorizableTaskQueryHandler(IAuthorizableTaskBuilder authorizableTaskBuilder)
        {
            _authorizableTaskBuilder = authorizableTaskBuilder;
        }

        /// <summary>
        /// Handles an <see cref="AuthorizableTaskQuery"/>
        /// </summary>
        public async Task<IAuthorizableTask> Handle(AuthorizableTaskQuery message)
        {
            return await _authorizableTaskBuilder.Build(message.TaskId, message.EventId, message.CampaignId, message.OrganizationId);
        }
    }
}
