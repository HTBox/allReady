using AllReady.Security;
using MediatR;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Organizations
{
    /// <summary>
    /// Uses an <see cref="IAuthorizableOrganizationBuilder"/> to build a <see cref="IAuthorizableOrganization"/>
    /// </summary>
    public class AuthorizableOrganizationQuery : IAsyncRequest<IAuthorizableOrganization>
    {
        /// <summary>
        /// Initializes a new instance of an <see cref="AuthorizableOrganizationQuery"/>.
        /// Uses an <see cref="IAuthorizableOrganizationBuilder"/> to build a <see cref="IAuthorizableOrganization"/>
        /// </summary>
        public AuthorizableOrganizationQuery(int organizationId)
        {
            OrganizationId = organizationId;
        }

        /// <summary>
        /// The organization ID
        /// </summary>
        public int OrganizationId { get; }
    }

    /// <summary>
    /// Handles an <see cref="AuthorizableOrganizationQuery"/>
    /// </summary>
    public class AuthorizableOrganizationQueryHandler : IAsyncRequestHandler<AuthorizableOrganizationQuery, IAuthorizableOrganization>
    {
        private readonly IAuthorizableOrganizationBuilder _authorizableOrganizationBuilder;

        public AuthorizableOrganizationQueryHandler(IAuthorizableOrganizationBuilder authorizableOrganizationBuilder)
        {
            _authorizableOrganizationBuilder = authorizableOrganizationBuilder;
        }

        /// <summary>
        /// Handles an <see cref="AuthorizableOrganizationQuery"/>
        /// </summary>
        public async Task<IAuthorizableOrganization> Handle(AuthorizableOrganizationQuery message)
        {
            return await _authorizableOrganizationBuilder.Build(message.OrganizationId);
        }
    }
}
