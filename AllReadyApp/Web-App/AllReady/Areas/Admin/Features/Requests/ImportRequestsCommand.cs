using MediatR;
using System.Collections.Generic;
using AllReady.Areas.Admin.ViewModels.Import;

namespace AllReady.Areas.Admin.Features.Requests
{
    public class ImportRequestsCommand : IAsyncRequest
    {
        public int EventId { get; set; }
        public List<ImportRequestViewModel> ImportRequestViewModels { get; set; }
    }
}
