using MediatR;

namespace AllReady.Areas.Admin.Features.Site
{
    public class SendAccountApprovalEmailCommand : IAsyncRequest
    {
        public string Email { get; set; }
        public string CallbackUrl { get; set; }
    }
}
