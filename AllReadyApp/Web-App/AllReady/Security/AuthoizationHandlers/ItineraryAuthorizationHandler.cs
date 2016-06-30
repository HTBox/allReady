using System;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Organizations;
using AllReady.Models;
using MediatR;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Authorization.Infrastructure;

namespace AllReady.Security.AuthoizationHandlers
{
    /// <summary>
    /// Authorizes access to itinerary objects by establishing the organization id
    /// </summary>
    public class ItineraryAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, ItineraryAuthorizationStub>
    {
        private readonly IMediator _mediator;

        public ItineraryAuthorizationHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        protected override void Handle(AuthorizationContext context, OperationAuthorizationRequirement requirement, ItineraryAuthorizationStub resource)
        {
            // note: sgordon: I believe in RTM this will be cleaner as the AuthorizationHandler has been made Async only so we shouldn't have to override this sync method
            throw new NotImplementedException();
        }

        protected override async Task HandleAsync(AuthorizationContext context, OperationAuthorizationRequirement requirement, ItineraryAuthorizationStub resource)
        {
            // todo: sgordon: We should probably look to implement an in memory cache within the mediator handler as this could be called quite often as we grow the number of admins and orgs
            var orgId = await _mediator.SendAsync(new OrganizationIdQuery { ItineraryId = resource.ItineraryId });

            if (!context.User.IsOrganizationAdmin(orgId))
            {
                context.Fail();
                return;
            }

            context.Succeed(requirement);
        }
    }
}
