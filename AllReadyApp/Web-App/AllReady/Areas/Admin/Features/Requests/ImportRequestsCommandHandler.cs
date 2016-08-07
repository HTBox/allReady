using AllReady.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Requests
{
    public class ImportRequestsCommandHandler : IRequestHandler<ImportRequestsCommand, IEnumerable<ImportRequestError>>
    {
        private readonly AllReadyContext _context;

        public ImportRequestsCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        public IEnumerable<ImportRequestError> Handle(ImportRequestsCommand message)
        {
            var errors = new List<ImportRequestError>();

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
