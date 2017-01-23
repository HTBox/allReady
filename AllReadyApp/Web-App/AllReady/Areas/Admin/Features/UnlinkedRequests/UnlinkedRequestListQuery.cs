using System.Collections.Generic;
using AllReady.Areas.Admin.ViewModels.UnlinkedRequests;
using MediatR;

namespace AllReady.Areas.Admin.Features.UnlinkedRequests
{
    public class UnlinkedRequestListQuery : IAsyncRequest<List<UnlinkedRequestViewModel>>
    {
        public int OrganizationId { get; set; }
    }
}
