using AllReady.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Requests
{
    public class ImportRequestsCommand : IRequest<IEnumerable<ImportRequestError>>
    {
        public List<Request> Requests { get; set; }
    }
}
