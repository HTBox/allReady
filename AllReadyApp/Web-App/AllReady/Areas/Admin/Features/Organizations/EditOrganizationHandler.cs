using System.Threading.Tasks;
using AllReady.Areas.Admin.Extensions;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class EditOrganizationHandler : IAsyncRequestHandler<EditOrganization, int>
    {
        private readonly AllReadyContext _context;

        public EditOrganizationHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(EditOrganization message)
        {
            var org = await _context.Organizations
                .Include(l => l.Location)
                .Include(tc => tc.OrganizationContacts)
                .SingleOrDefaultAsync(t => t.Id == message.Organization.Id) ?? _context.Add(new Organization()).Entity;

            org.Name = message.Organization.Name;
            org.LogoUrl = message.Organization.LogoUrl;
            org.WebUrl = message.Organization.WebUrl;

            org.DescriptionHtml = message.Organization.Description;
            org.Summary = message.Organization.Summary;

            //TODO: mgmccarthy: pull code from ContactExtension.UpdateOrganizationContact into this handler as this the only code that uses it and it should not be in an extension method... espeically doing database access
            org = await org.UpdateOrganizationContact(message.Organization, _context);
            org.Location = org.Location.UpdateModel(message.Organization.Location);
            org.Location.PostalCode = message.Organization.Location.PostalCode;

            org.PrivacyPolicy = message.Organization.PrivacyPolicy;
            org.PrivacyPolicyUrl = message.Organization.PrivacyPolicyUrl;

            await _context.SaveChangesAsync();

            return org.Id;
        }       
    }
}
