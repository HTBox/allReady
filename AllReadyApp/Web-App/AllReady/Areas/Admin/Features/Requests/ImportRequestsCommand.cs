using AllReady.Models;
using MediatR;
using System.Collections.Generic;

namespace AllReady.Areas.Admin.Features.Requests
{
    public class ImportRequestsCommand : IRequest<IEnumerable<ImportRequestError>>
    {
        public List<Request> Requests { get; set; }
    }
}
