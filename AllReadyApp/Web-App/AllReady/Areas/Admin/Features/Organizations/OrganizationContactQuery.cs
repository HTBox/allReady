using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationContactQuery : IRequest<ContactInformationModel>
    {
        public int Id { get; set; }
        public ContactTypes ContactType { get; set; } = ContactTypes.Primary;
    }
}