using AllReady.Security;
using MediatR;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Events
{
    /// <summary>
    /// Gets an <see cref="IAuthorizableEvent"/> and uses it to return whether the current user has access to manage it
    /// </summary>
    public class AuthorizableEventIsUserAuthorizedQuery : IAsyncRequest<bool>
    {
        /// <summary>
        /// Initializes a new instance of an <see cref="AuthorizableEventIsUserAuthorizedQuery"/>.
        /// Gets an <see cref="IAuthorizableEvent"/> and uses it to return whether the current user has access to manage it
        /// </summary>
        public AuthorizableEventIsUserAuthorizedQuery(int eventId, int? campaignId = null, int? orgId = null)
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
    /// Handles an <see cref="AuthorizableEventIsUserAuthorizedQuery"/>
    /// </summary>
    public class AuthorizableEventIsUserAuthorizedQueryHandler : IAsyncRequestHandler<AuthorizableEventIsUserAuthorizedQuery, bool>
    {
        private readonly IAuthorizableEventBuilder _authorizableEventBuilder;

        public AuthorizableEventIsUserAuthorizedQueryHandler(IAuthorizableEventBuilder authorizableEventBuilder)
        {
            _authorizableEventBuilder = authorizableEventBuilder;
        }

        /// <summary>
        /// Handles an <see cref="AuthorizableEventIsUserAuthorizedQuery"/>
        /// </summary>
        public async Task<bool> Handle(AuthorizableEventIsUserAuthorizedQuery message)
        {
            var authorizableEvent = await _authorizableEventBuilder.Build(message.EventId, message.CampaignId, message.OrganizationId);

            return await authorizableEvent.IsUserAuthorized();
        }
    }
}
