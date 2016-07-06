using AllReady.Areas.Admin.Features.Organizations;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using System.Linq;
using Xunit;
using System;

namespace AllReady.UnitTest.Areas.Admin.Features.Organizations
{
    public class OrganizationEditCommandHandlerTests : InMemoryContextTest
    {
        [Fact(Skip = "RTM Broken Tests")]
        public void AddOrganization()
        {
            // Arrange
            Organization org = CreateAgincourtAwareness();
            OrganizationEditModel orgModel = OrganizationEditCommandHandlerTests.ToEditModel_Organization(org);

            // Act
            OrganizationEditCommandHandler handler = new OrganizationEditCommandHandler(Context);
            int id = handler.Handle(new OrganizationEditCommand() { Organization = orgModel });
            Organization fetchedOrg = Context.Organizations.Where(t => t.Id == id).First();

            // Assert
            Assert.Single(Context.Organizations.Where(t => t.Id == id));
            Assert.Equal(Context.Entry(fetchedOrg).State, Microsoft.EntityFrameworkCore.EntityState.Unchanged);
        }

        protected override void LoadTestData()
        {
        }

        public static Location Create25Bogus()
        {
            return new Location() { Address1 = "25 Bogus Ave", City = "Agincourt", State = "Ontario", Country = "Canada", PostalCode = "M1T2T9" };
        }
        public static Organization CreateAgincourtAwareness()
        {
            return new Organization() { Name = "Agincourt Awareness", Location = Create25Bogus(), WebUrl = "http://www.AgincourtAwareness.ca", LogoUrl = "http://www.AgincourtAwareness.ca/assets/LogoLarge.png" };
        }
        /// <summary>
        /// Creates an OrganizationEditModel from an existing Organization 
        /// </summary>
        /// <param name="organization">An Organization that will be supply the initial values for editing.</param>
        /// <returns>An OrganizationEditModel that can be used to capture the fields for an Organization
        /// and it's location and primary contact built from values from the supplied organization. </returns>
        public static OrganizationEditModel ToEditModel_Organization(Organization organization)
        {
            OrganizationEditModel ret = new OrganizationEditModel()
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
                ret = (OrganizationEditModel)organization.OrganizationContacts?.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary)?.Contact.ToEditModel(ret);
            }

            return ret;
        }
    }
}
