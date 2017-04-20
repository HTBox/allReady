using AllReady.Security;
using MediatR;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    /// <summary>
    /// Uses an <see cref="IAuthorizableCampaignBuilder"/> to build a <see cref="IAuthorizableCampaign"/>
    /// </summary>
    public class AuthorizableCampaignQuery : IAsyncRequest<IAuthorizableCampaign>
    {
        /// <summary>
        /// Initializes a new instance of an <see cref="AuthorizableCampaignQuery"/>.
        /// Uses an <see cref="IAuthorizableCampaignBuilder"/> to build a <see cref="IAuthorizableCampaign"/>
        /// </summary>
        public AuthorizableCampaignQuery(int campaignId, int? orgId = null)
        {
            CampaignId = campaignId;
            OrganizationId = orgId;
        }

        /// <summary>
        /// The campaign ID
        /// </summary>
        public int CampaignId { get; }

        /// <summary>
        /// The organization ID for the campaign, if known
        /// </summary>
        public int? OrganizationId { get; }
    }

    /// <summary>
    /// Handles an <see cref="AuthorizableCampaignQuery"/>
    /// </summary>
    public class AuthorizableCampaignQueryHandler : IAsyncRequestHandler<AuthorizableCampaignQuery, IAuthorizableCampaign>
    {
        private readonly IAuthorizableCampaignBuilder _authorizableCampaignBuilder;

        public AuthorizableCampaignQueryHandler(IAuthorizableCampaignBuilder authorizableCampaignBuilder)
        {
            _authorizableCampaignBuilder = authorizableCampaignBuilder;
        }

        /// <summary>
        /// Handles an <see cref="AuthorizableCampaignQuery"/>
        /// </summary>
        public async Task<IAuthorizableCampaign> Handle(AuthorizableCampaignQuery message)
        {
            return await _authorizableCampaignBuilder.Build(message.CampaignId, message.OrganizationId);
        }
    }
}
