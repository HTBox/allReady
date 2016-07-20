using AllReady.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Requests
{
    public class AddRequestsCommand : IRequest<IEnumerable<RequestImportError>>
    {
        public List<Request> Requests { get; set; }
    }
}
