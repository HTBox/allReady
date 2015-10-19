using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Features.Notifications
{
    public class QueuedSmsMessage
    {
        public string Recipient { get; set; }
        public string Message { get; set; }
    }
}
