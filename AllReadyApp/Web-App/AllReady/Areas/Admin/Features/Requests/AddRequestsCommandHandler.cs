using AllReady.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Requests
{
    public class AddRequestsCommandHandler : IRequestHandler<AddRequestsCommand, IEnumerable<RequestImportError>>
    {
        private readonly AllReadyContext _context;

        public AddRequestsCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        public IEnumerable<RequestImportError> Handle(AddRequestsCommand message)
        {
            var errors = new List<RequestImportError>();

            foreach (var request in message.Requests)
            {
                // todo: do basic data validation
                if(!_context.Requests.Any(r => r.ProviderId == request.ProviderId))
                    _context.Requests.Add(request);
            }
            _context.SaveChanges();

            return errors;
        }

    }
}
