using System.Collections.Generic;
using AllReady.Areas.Admin.ViewModels.OrganizationApi;
using MediatR;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationListQueryAysnc : IAsyncRequest<List<OrganizationSummaryViewModel>>
    {
    }
}