using AllReady.Areas.Admin.Features.Organizations;
using AllReady.Models;
using System.Linq;
using Xunit;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Extensions;
using AllReady.Areas.Admin.ViewModels.Organization;

namespace AllReady.UnitTest.Areas.Admin.Features.Organizations
{
    public class EditOrganizationCommandHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task AddOrganization()
        {
            // Arrange
            var org = CreateAgincourtAwareness();
            var orgModel = ToEditModel_Organization(org);

            // Act
            var handler = new EditOrganizationCommandHandler(Context);
            var id = await handler.Handle(new EditOrganizationCommand { Organization = orgModel });
            var fetchedOrg = Context.Organizations.First(t => t.Id == id);

            // Assert
            Assert.Single(Context.Organizations.Where(t => t.Id == id));
            Assert.Equal(Microsoft.EntityFrameworkCore.EntityState.Unchanged, Context.Entry(fetchedOrg).State);
        }

        public static Location Create25Bogus()
        {
            return new Location { Address1 = "25 Bogus Ave", City = "Agincourt", State = "Ontario", Country = "Canada", PostalCode = "M1T2T9" };
        }
        public static Organization CreateAgincourtAwareness()
        {
            return new Organization { Name = "Agincourt Awareness", Location = Create25Bogus(), WebUrl = "http://www.AgincourtAwareness.ca", LogoUrl = "http://www.AgincourtAwareness.ca/assets/LogoLarge.png" };
        }

        /// <summary>
        /// Creates an OrganizationEditModel from an existing Organization 
        /// </summary>
        /// <param name="organization">An Organization that will be supply the initial values for editing.</param>
        /// <returns>An OrganizationEditModel that can be used to capture the fields for an Organization
        /// and it's location and primary contact built from values from the supplied organization. </returns>
        public static OrganizationEditViewModel ToEditModel_Organization(Organization organization)
        {
            var ret = new OrganizationEditViewModel
            {
                Id = organization.Id,
                Name = organization.Name,
                Location = organization.Location.ToEditModel(),
                LogoUrl = organization.LogoUrl,
                WebUrl = organization.WebUrl,
                PrivacyPolicy = organization.PrivacyPolicy
            };

            if (organization.OrganizationContacts?.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary)?.Contact != null)
            {
                ret = (OrganizationEditViewModel)organization.OrganizationContacts?.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary)?.Contact.ToEditModel(ret);
            }

            return ret;
        }
    }
}