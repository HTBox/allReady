using System;
using System.Linq;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.Requests
{
    public class AddRequestCommandHandler : IRequestHandler<AddRequestCommand, AddRequestError>
    {
        private readonly AllReadyContext _context;

        public AddRequestCommandHandler( AllReadyContext context )
        {
            _context = context;
        }

        public AddRequestError Handle( AddRequestCommand message )
        {
            AddRequestError error = null;
            var request = message.Request;

            try
            {
                //todo: I'm not sure if this logic is going to be correct, as this allows an update of status to existing requests.  I added this because the red cross is passing in current status.
                Request entity = _context.Requests.FirstOrDefault(r => r.ProviderId == request.ProviderId);
                if (entity == null)
                {
                    request.RequestId = Guid.NewGuid();
                    _context.Requests.Add(request);
                }
                else
                {
                    entity.Status = request.Status;
                }

                _context.SaveChanges();
            }
            catch (Exception)
            {
                error = new AddRequestError
                {
                    ProviderId = message.Request.ProviderId,
                    Reason = "Failed to add request."
                };

                //todo: Logging for this error
            }

            return error;
        }

    }
}
