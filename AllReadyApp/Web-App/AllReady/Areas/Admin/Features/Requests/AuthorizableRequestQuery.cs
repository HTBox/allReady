using AllReady.Security;
using MediatR;
using System;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Requests
{
    /// <summary>
    /// Uses an <see cref="IAuthorizableRequestBuilder"/> to build a <see cref="IAuthorizableRequest"/>
    /// </summary>
    public class AuthorizableRequestQuery : IAsyncRequest<IAuthorizableRequest>
    {
        /// <summary>
        /// Initializes a new instance of an <see cref="AuthorizableRequestQuery"/>.
        /// Uses an <see cref="IAuthorizableRequestBuilder"/> to build a <see cref="IAuthorizableRequest"/>
        /// </summary>
        public AuthorizableRequestQuery(Guid requestId, int? eventId = null, int? campaignId = null, int? orgId = null)
        {
            RequestId = requestId;
            EventId = eventId;
            CampaignId = campaignId;
            OrganizationId = orgId;
        }

        /// <summary>
        /// The request ID
        /// </summary>
        public Guid RequestId { get; }

        /// <summary>
        /// The event ID for the request, if known
        /// </summary>
        public int? EventId { get; }

        /// <summary>
        /// The organization ID for the request, if known
        /// </summary>
        public int? OrganizationId { get; }

        /// <summary>
        /// The campaign ID for the request, if known
        /// </summary>
        public int? CampaignId { get; }
    }

    /// <summary>
    /// Handles an <see cref="AuthorizableRequestQuery"/>
    /// </summary>
    public class AuthorizableRequestQueryHandler : IAsyncRequestHandler<AuthorizableRequestQuery, IAuthorizableRequest>
    {
        private readonly IAuthorizableRequestBuilder _authorizableRequestBuilder;

        public AuthorizableRequestQueryHandler(IAuthorizableRequestBuilder authorizableRequestBuilder)
        {
            _authorizableRequestBuilder = authorizableRequestBuilder;
        }

        /// <summary>
        /// Handles an <see cref="AuthorizableRequestQuery"/>
        /// </summary>
        public async Task<IAuthorizableRequest> Handle(AuthorizableRequestQuery message)
        {
            return await _authorizableRequestBuilder.Build(message.RequestId, message.EventId, message.CampaignId, message.OrganizationId);
        }
    }
}
