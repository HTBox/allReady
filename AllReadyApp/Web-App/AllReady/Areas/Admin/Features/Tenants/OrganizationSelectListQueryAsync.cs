using MediatR;
using Microsoft.AspNet.Mvc.Rendering;
using System.Collections.Generic;

namespace AllReady.Areas.Admin.Features.Tenants
{
    public class OrganizationSelectListQueryAsync: IAsyncRequest<IEnumerable<SelectListItem>>
    {
    }
}