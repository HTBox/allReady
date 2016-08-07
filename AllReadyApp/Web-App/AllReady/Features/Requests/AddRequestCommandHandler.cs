using System;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.Requests
{
    public class AddRequestCommandHandler : IAsyncRequestHandler<AddRequestCommand, AddRequestError>
    {
        private readonly IAllReadyDataAccess _dataAccess;

        public AddRequestCommandHandler( IAllReadyDataAccess dataAccess )
        {
            _dataAccess = dataAccess;
        }

        public async Task<AddRequestError> Handle( AddRequestCommand message )
        {
            AddRequestError error = null;
            if (message.Request == null)
            {
                throw new InvalidOperationException("Request property is required.");
            }

            var request = message.Request;

            try
            {
                //todo: I'm not sure if this logic is going to be correct, as this allows an update of status to existing requests.  I added this because the red cross is passing in current status.
                Request entity = await _dataAccess.GetRequestByProviderIdAsync(message.Request?.ProviderId);

                if (entity == null)
                {
                    request.RequestId = Guid.NewGuid();
                }
                else
                {
                    entity.Status = request.Status;
                }

                await _dataAccess.AddRequestAsync(entity);
            }
            catch (Exception)
            {
                error = new AddRequestError
                {
                    ProviderId = message.Request?.ProviderId ?? "No ProviderId.",
                    Reason = "Failed to add request."
                };

                //todo: Logging for this error
            }

            return error;
        }

    }
}
