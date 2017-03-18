using AllReady.Security;
using MediatR;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Events
{
    public class AuthorizableEventQuery : IAsyncRequest<bool>
    {
        public AuthorizableEventQuery(int eventId, int? campaignId = null, int? orgId = null)
        {
            EventId = eventId;
            CampaignId = campaignId;
            OrganizationId = orgId;
        }

        public int EventId { get; }
        public int? OrganizationId { get; }
        public int? CampaignId { get; }
    }

    public class AuthorizableEventQueryHandler : IAsyncRequestHandler<AuthorizableEventQuery, bool>
    {
        private readonly IAuthorizableEventBuilder _authorizableEventBuilder;

        public AuthorizableEventQueryHandler(IAuthorizableEventBuilder authorizableEventBuilder)
        {
            _authorizableEventBuilder = authorizableEventBuilder;
        }

        public async Task<bool> Handle(AuthorizableEventQuery message)
        {
            var authorizableEvent = await _authorizableEventBuilder.Build(message.EventId, message.CampaignId, message.OrganizationId);

            return await authorizableEvent.UserIsAuthorized();
        }
    }
}
