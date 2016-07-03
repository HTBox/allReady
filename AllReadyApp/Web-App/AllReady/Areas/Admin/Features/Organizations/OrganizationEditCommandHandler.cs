using System.Linq;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationEditCommandHandler : IRequestHandler<OrganizationEditCommand, int>
    {
        private AllReadyContext _context;

        public OrganizationEditCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        public int Handle(OrganizationEditCommand message)
        {
            var organization = _context
                    .Organizations
                    .Include(l => l.Location)
                    .Include(tc => tc.OrganizationContacts)
                    .SingleOrDefault(t => t.Id == message.Organization.Id);

            if (organization == null)
            {
                organization = new Organization();
            }

            organization.Name = message.Organization.Name;
            organization.LogoUrl = message.Organization.LogoUrl;
            organization.WebUrl = message.Organization.WebUrl;

            organization = organization.UpdateOrganizationContact(message.Organization, _context);
            organization.Location = organization.Location.UpdateModel(message.Organization.Location);
            organization.Location.PostalCode = message.Organization.Location.PostalCode;

            organization.PrivacyPolicy = message.Organization.PrivacyPolicy;

            _context.Update(organization);
            _context.SaveChanges();

            return organization.Id;
        }       
    }
}
