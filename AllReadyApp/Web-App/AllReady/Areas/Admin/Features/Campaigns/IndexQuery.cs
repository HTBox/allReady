using System.Collections.Generic;
using AllReady.Areas.Admin.ViewModels.Campaign;
using MediatR;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class IndexQuery : IAsyncRequest<IEnumerable<IndexViewModel>>
    {
        public int? OrganizationId { get; set; }
    }
}