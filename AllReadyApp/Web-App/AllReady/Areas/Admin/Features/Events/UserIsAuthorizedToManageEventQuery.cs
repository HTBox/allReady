using AllReady.Security;
using MediatR;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Events
{
    /// <summary>
    /// Builds an <see cref="IAuthorizableEvent"/> and uses it to determine whether the current user has authorization to manage it
    /// </summary>
    public class UserIsAuthorizedToManageEventQuery : IAsyncRequest<bool>
    {
        /// <summary>
        /// Initializes a new instance of an <see cref="UserIsAuthorizedToManageEventQuery"/>.
        /// Gets an <see cref="IAuthorizableEvent"/> and uses it to return whether the current user has access to manage it
        /// </summary>
        public UserIsAuthorizedToManageEventQuery(int eventId, int? campaignId = null, int? orgId = null)
        {
            EventId = eventId;
            CampaignId = campaignId;
            OrganizationId = orgId;
        }

        /// <summary>
        /// The event ID
        /// </summary>
        public int EventId { get; }

        /// <summary>
        /// The organization ID for the event, if known
        /// </summary>
        public int? OrganizationId { get; }

        /// <summary>
        /// The campaign ID for the event, if known
        /// </summary>
        public int? CampaignId { get; }
    }

    /// <summary>
    /// Handles an <see cref="UserIsAuthorizedToManageEventQuery"/>
    /// </summary>
    public class UserIsAuthorizedToManageEventQueryHandler : IAsyncRequestHandler<UserIsAuthorizedToManageEventQuery, bool>
    {
        private readonly IAuthorizableEventBuilder _authorizableEventBuilder;

        public UserIsAuthorizedToManageEventQueryHandler(IAuthorizableEventBuilder authorizableEventBuilder)
        {
            _authorizableEventBuilder = authorizableEventBuilder;
        }

        /// <summary>
        /// Handles an <see cref="UserIsAuthorizedToManageEventQuery"/>
        /// </summary>
        public async Task<bool> Handle(UserIsAuthorizedToManageEventQuery message)
        {
            var authorizableEvent = await _authorizableEventBuilder.Build(message.EventId, message.CampaignId, message.OrganizationId);

            return await authorizableEvent.IsUserAuthorized();
        }
    }
}
