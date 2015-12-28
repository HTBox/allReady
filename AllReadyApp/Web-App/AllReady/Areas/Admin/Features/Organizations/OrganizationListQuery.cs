using AllReady.Areas.Admin.Models;
using MediatR;
using System.Collections.Generic;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationListQuery : IRequest<IEnumerable<OrganizationSummaryModel>>
    {
    }
}