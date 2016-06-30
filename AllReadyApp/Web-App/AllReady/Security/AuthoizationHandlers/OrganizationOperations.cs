using Microsoft.AspNet.Authorization.Infrastructure;

namespace AllReady.Security.AuthoizationHandlers
{
    public static class OrganizationOperations
    {
        public static OperationAuthorizationRequirement Manage = new OperationAuthorizationRequirement { Name = "Manage" };
    }
}
