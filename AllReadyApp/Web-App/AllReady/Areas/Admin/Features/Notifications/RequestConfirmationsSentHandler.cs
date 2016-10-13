using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace AllReady.Areas.Admin.Features.Notifications
{
    public class RequestConfirmationsSentHandler //: IAsyncNotificationHandler<RequestConfirmationsSent>
    {
        //public RequestConfirmationsSentHandler()
        //{
        //}

        //public async Task Handle(RequestConfirmationsSent notification)
        //{
        //    //the sms's have been sent out for requestor's to confirm or cancel their request... what we need to do here
        //    //1. query the Requet table and check to see if they status has been moved to ??? for each request
        //    //2. if the status has not been moved to the ??? status that means they've confirmed, if it's been moved to unassigned, then they've canceled, if it's "assigned" then they have not got back to us yet
        //    //3. if they have not got back to us yet (which is doubtful for this short time), schedule a job a week before the Request's DateAdded value to send another confirm/cancel sms text
        //    //- send a command to do this
        //    //- BackgroundJob.Schedule(() => TextBuffer.WriteLine("Scheduled Job completed successfully!"), TimeSpan.FromSeconds(5));
        //    //- is there any way to have the Schedule lambda actually send a message via a mediator so we can designate a handler for the "week before" check?
        //    //- this handler will again query the db to see what the status of the request is and either do nothing, or enqueue another scheduled job
        //    //- then same thing for day before request, y/n sms text
        //}
    }
}
