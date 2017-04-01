using AllReady.Security;
using MediatR;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Events
{
    /// <summary>
    /// Uses an <see cref="IAuthorizableEventBuilder"/> to build a <see cref="IAuthorizableEvent"/>
    /// </summary>
    public class AuthorizableEventQuery : IAsyncRequest<IAuthorizableEvent>
    {
        /// <summary>
        /// Initializes a new instance of an <see cref="AuthorizableEventQuery"/>.
        /// Uses an <see cref="IAuthorizableEventBuilder"/> to build a <see cref="IAuthorizableEvent"/>
        /// </summary>
        public AuthorizableEventQuery(int eventId, int? campaignId = null, int? orgId = null)
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
    /// Handles an <see cref="AuthorizableEventQuery"/>
    /// </summary>
    public class AuthorizableEventQueryHandler : IAsyncRequestHandler<AuthorizableEventQuery, IAuthorizableEvent>
    {
        private readonly IAuthorizableEventBuilder _authorizableEventBuilder;

        public AuthorizableEventQueryHandler(IAuthorizableEventBuilder authorizableEventBuilder)
        {
            _authorizableEventBuilder = authorizableEventBuilder;
        }

        /// <summary>
        /// Handles an <see cref="AuthorizableEventQuery"/>
        /// </summary>
        public async Task<IAuthorizableEvent> Handle(AuthorizableEventQuery message)
        {
            return await _authorizableEventBuilder.Build(message.EventId, message.CampaignId, message.OrganizationId);
        }
    }
}
