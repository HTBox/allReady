using MediatR;
using System.Collections.Generic;
using AllReady.Areas.Admin.ViewModels.Import;

namespace AllReady.Areas.Admin.Features.Requests
{
    public class ImportRequestsCommand : IRequest
    {
        public List<ImportRequestViewModel> ImportRequestViewModels { get; set; }
    }
}
