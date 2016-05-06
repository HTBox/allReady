using System.Collections.Generic;
using AllReady.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class AllOrganizationsQuery : IRequest<IEnumerable<Organization>>
    {
    }
}
