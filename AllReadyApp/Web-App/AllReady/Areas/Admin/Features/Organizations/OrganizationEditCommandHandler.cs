using System.Linq;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationEditCommandHandler : IRequestHandler<OrganizationEditCommand, int>
    {
        private readonly AllReadyContext _context;

        public OrganizationEditCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        public int Handle(OrganizationEditCommand message)
        {
            var org = _context
                .Organizations
                .Include(l => l.Location)
                .Include(tc => tc.OrganizationContacts)
                .SingleOrDefault(t => t.Id == message.Organization.Id) ?? new Organization();

            org.Name = message.Organization.Name;
            org.LogoUrl = message.Organization.LogoUrl;
            org.WebUrl = message.Organization.WebUrl;

            org.DescriptionHtml = message.Organization.Description;
            org.Summary = message.Organization.Summary;

            org = org.UpdateOrganizationContact(message.Organization, _context);
            org.Location = org.Location.UpdateModel(message.Organization.Location);
            org.Location.PostalCode = message.Organization.Location.PostalCode;

            org.PrivacyPolicy = message.Organization.PrivacyPolicy;

            _context.Update(org);
            _context.SaveChanges();

            return org.Id;
        }       
    }
}
