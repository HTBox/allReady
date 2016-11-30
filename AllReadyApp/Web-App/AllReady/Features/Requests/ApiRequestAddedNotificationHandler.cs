using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Features.Requests
{
    public class ApiRequestAddedNotificationHandler : IAsyncNotificationHandler<ApiRequestAddedNotification>
    {
        private readonly AllReadyContext context;
        private readonly IMediator mediator;

        public ApiRequestAddedNotificationHandler(AllReadyContext context, IMediator mediator)
        {
            this.context = context;
            this.mediator = mediator;
        }

        public async Task Handle(ApiRequestAddedNotification notification)
        {
            //TODO mgmccarthy: look up if we can serivce this request or not by region/zip code, and grab the resulting true/false that will be returned on the acceptance
            //field of getasmokealarm's API invocation

            var request = await context.Requests.SingleAsync(x => x.RequestId == notification.RequestId);

            //TODO: onece we have looked up the ability to service the request, send a command to invoke getasmokealarm's API using Polly
            await mediator.SendAsync(new SendRequestStatusToGetASmokeAlarmEndpoint { Serial = request.ProviderId, Acceptance = true, Status = "new" });
        }
    }
}
