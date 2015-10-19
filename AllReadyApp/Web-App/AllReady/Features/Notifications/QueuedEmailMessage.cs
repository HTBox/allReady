using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Features.Notifications
{
    public class QueuedEmailMessage
    {
        public string Recipient { get; set; }
        public string Message { get; set; }
        // todo: augment this class to support mail service templates and/or html messages
    }
}
