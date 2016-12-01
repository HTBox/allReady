using System;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Extensions;
using AllReady.Models;
using MediatR;
using Geocoding;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Features.Requests
{
    public class ProcessApiRequestCommandHandler : AsyncRequestHandler<ProcessApiRequestCommand>
    {
        private readonly AllReadyContext context;
        private readonly IGeocoder geocoder;
        private readonly IMediator mediator;

        public Func<Guid> NewRequestId = () => Guid.NewGuid();
        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        public ProcessApiRequestCommandHandler(AllReadyContext context, IGeocoder geocoder, IMediator mediator)
        {
            this.context = context;
            this.geocoder = geocoder;
            this.mediator = mediator;
        }

        protected override async Task HandleCore(ProcessApiRequestCommand message)
        {
            //This is WIP code. What I'm going to do for now is take the first matching zip code I can find on an Event for this incoming Request, and if a match is made, I'll create
            //the Request with that EventId
            var matchingEventByZipCode = await context.Events
                .Include(e => e.Location)
                .FirstOrDefaultAsync(e => e.Location.PostalCode == message.ViewModel.Zip);

            if (matchingEventByZipCode != null)
            {
                var request = new Request
                {
                    RequestId = NewRequestId(),
                    ProviderId = message.ViewModel.ProviderRequestId,
                    ProviderData = message.ViewModel.ProviderData,
                    Address = message.ViewModel.Address,
                    City = message.ViewModel.City,
                    DateAdded = DateTimeUtcNow(),
                    Email = message.ViewModel.Email,
                    Name = message.ViewModel.Name,
                    Phone = message.ViewModel.Phone,
                    State = message.ViewModel.State,
                    Zip = message.ViewModel.Zip,
                    Status = RequestStatus.Unassigned,
                    Source = RequestSource.Api,
                    EventId = matchingEventByZipCode.Id
                };

                var address = geocoder.Geocode(message.ViewModel.Address, message.ViewModel.City, message.ViewModel.State, message.ViewModel.Zip, string.Empty).FirstOrDefault();
                request.Latitude = address?.Coordinates.Latitude ?? 0;
                request.Longitude = address?.Coordinates.Longitude ?? 0;

                context.AddOrUpdate(request);
                await context.SaveChangesAsync();
            }

            //TODO mgmccarthy: instead of using mediator here, we should instead use Hangfire to Enqueue a job that would contain the code in ApiRequestProcessedNotificationHandler
            //this allows the thread to return back to RequestApiController immediately after we've successfully written the new Request to the database, hence, 
            //finishing the RPC from getasmokealarm's request
            //OR
            //we instead using Hangfire from RequestApiController to immediately return a 202 to getasmokealarm, and asynchronously create the Request and keep using the mediator
            //for the rest of the pipeline up until getasmokealarm's API invocation through Polly in SendRequestStatusToGetASmokeAlarmHandler
            //still need to decide whic his best
            await mediator.PublishAsync(new ApiRequestProcessedNotification { ProviderRequestId = message.ViewModel.ProviderRequestId });
        }
    }
}