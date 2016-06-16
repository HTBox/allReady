using System.Collections.Generic;
using AllReady.ViewModels;
using MediatR;

namespace AllReady.Features.Organizations
{
    public class OrganizationsQuery : IRequest<List<OrganizationViewModel>>
    {
    }
}
