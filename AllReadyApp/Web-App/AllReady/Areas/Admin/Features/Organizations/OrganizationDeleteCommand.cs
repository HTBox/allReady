using MediatR;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationDeleteCommand : IRequest
    {
        public int Id { get; set; }
    }
}
