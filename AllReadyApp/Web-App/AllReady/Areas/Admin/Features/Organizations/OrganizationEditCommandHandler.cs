using System.Linq;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;

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
                    .Include(l => l.Location).ThenInclude(p => p.PostalCode)
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

            if (organization.Location != null)
            {
                if (!string.IsNullOrWhiteSpace(message.Organization.Location.PostalCode))
                {
                    PostalCodeGeo postalCode = _context.PostalCodes.SingleOrDefault(pc => pc.PostalCode.Equals(message.Organization.Location.PostalCode, System.StringComparison.InvariantCultureIgnoreCase));
                    if (postalCode == null)
                    {
                        postalCode = new PostalCodeGeo { PostalCode = message.Organization.Location.PostalCode, City = message.Organization.Location.City, State = message.Organization.Location.State };
                        _context.PostalCodes.Add(postalCode);
                    }
                    organization.Location.PostalCode = postalCode;
                }

                _context.Update(organization.Location);
            }

            organization.PrivacyPolicy = message.Organization.PrivacyPolicy;

            _context.Update(organization);
            _context.SaveChanges();

            return organization.Id;
        }       
    }
}
