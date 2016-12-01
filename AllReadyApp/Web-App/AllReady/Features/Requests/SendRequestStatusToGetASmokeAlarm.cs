using MediatR;

namespace AllReady.Features.Requests
{
    public class SendRequestStatusToGetASmokeAlarm : IAsyncRequest
    {
        public string Serial { get; set; }
        public bool Acceptance { get; set; }
        public string Status { get; set; }
    }
}
