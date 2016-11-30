using MediatR;

namespace AllReady.Features.Requests
{
    public class SendRequestStatusToGetASmokeAlarmEndpoint : IAsyncRequest
    {
        public string Serial { get; set; }
        public bool Acceptance { get; set; }
        public string Status { get; set; }
    }
}
