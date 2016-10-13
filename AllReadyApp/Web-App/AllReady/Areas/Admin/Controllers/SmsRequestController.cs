using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AllReady.Areas.Admin.Controllers
{
    public class SmsRequestController : Controller
    {
        //this action method assumes handling a confirmation or denial of a request
        public IActionResult Index()
        {
            //look up user's account by phone number
            //based on the RequestId (which we'll need to "ride" the sms message out to the user, then back with their resopnse), look up the status of the Request in Requests table
            //if they've confirmed, set the request to ???. Looks like AddRequestsCommandHandler already sets the Request's status to Assigned
            //- should be setting the Request's status to "assigned" before a user has confirmed/denied the request?
            //- do we need another status? Approved, etc....
            //if they've rescinded, set the request to Unassigned and remove the request from the ItineraryRequest table
        }
    }
}
