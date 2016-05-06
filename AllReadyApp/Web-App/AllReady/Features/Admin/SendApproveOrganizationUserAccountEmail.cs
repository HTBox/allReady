using MediatR;

namespace AllReady.Features.Admin
{
    public class SendApproveOrganizationUserAccountEmail : IAsyncRequest
    {
        public string DefaultAdminUsername { get; set; }
        public string CallbackUrl { get; set; }
    }
}
