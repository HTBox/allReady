using MediatR;

namespace AllReady.Features.Requests
{
    public class SendRequestStatusToRedCrossEndpoint : IAsyncRequest
    {
        public string SerialNumber { get; set; }
        public bool Acceptance { get; set; }
        public string Status { get; set; }
    }
}
