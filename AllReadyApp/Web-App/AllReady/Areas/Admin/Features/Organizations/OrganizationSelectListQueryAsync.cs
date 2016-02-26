using System.Collections.Generic;
using MediatR;
using Microsoft.AspNet.Mvc.Rendering;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationSelectListQueryAsync: IAsyncRequest<IEnumerable<SelectListItem>>
    {
    }
}