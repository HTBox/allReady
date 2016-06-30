using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Authorization.Infrastructure;

namespace AllReady.Security.AuthoizationHandlers
{
    /// <summary>
    /// Authorizes objects which implement the <seealso cref="IOrganizationIdAuthorizable"/> interface
    /// </summary>
    /// <remarks>As we have the OrganizationId on objects implementing the <seealso cref="IOrganizationIdAuthorizable"/> interface, no database calls are needed.</remarks>
    public class OrganizationIdAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, IOrganizationIdAuthorizable>
    {
        protected override void Handle(AuthorizationContext context, OperationAuthorizationRequirement requirement, IOrganizationIdAuthorizable resource)
        {
            if (!context.User.IsOrganizationAdmin(resource.OrganizationId))
            {
                context.Fail();
                return;
            }

            context.Succeed(requirement);
        }
    }
}