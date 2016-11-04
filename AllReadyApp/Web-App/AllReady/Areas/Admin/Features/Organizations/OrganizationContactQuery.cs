using AllReady.Areas.Admin.ViewModels.OrganizationApi;
using AllReady.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationContactQuery : IAsyncRequest<ContactInformationViewModel>
    {
        public int OrganizationId { get; set; }
        public ContactTypes ContactType { get; set; } = ContactTypes.Primary;
    }
}