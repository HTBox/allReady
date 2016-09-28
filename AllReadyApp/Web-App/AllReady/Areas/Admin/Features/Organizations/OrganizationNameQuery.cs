using MediatR;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationNameQuery : IAsyncRequest<string>
    {
        public int Id { get; set; }
    }
}