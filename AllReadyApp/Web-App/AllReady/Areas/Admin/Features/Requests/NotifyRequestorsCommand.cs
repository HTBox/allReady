using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Newtonsoft.Json;

namespace AllReady.Areas.Admin.Features.Requests
{
    public class NotifyRequestorsCommand : IAsyncRequest<bool>
    {
        //private Func<Request, Itinerary, string> _providedMessageBuilder;
        //public List<Request> Requests { get; set; }
        //public Itinerary Itinerary { get; set; }

        //public Func<Request, Itinerary, string> NotificationMessageBuilder
        //{
        //    get
        //    {
        //        //TODO: THis is wehere we will add the decision of message that needs to be serialized based on request communication preferences
        //        return ( r, i ) => JsonConvert.SerializeObject(new QueuedSmsMessage
        //        {
        //            Message = _providedMessageBuilder(r, i),
        //            Recipient = r.Phone.ToString()
        //        });
        //    }
        //    set { _providedMessageBuilder = value; }
        //}
    }
}
