using AllReady.Areas.Admin.Features.Organizations;
using AllReady.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Organizations
{
    public class OrganizationDetailQueryHandlerShould : InMemoryContextTest
    {
        protected override void LoadTestData()
        {
            var organization = new Organization
            {
                Id = 1,
                Name = "Org 1",
                WebUrl = "http://www.org1.org",
                LogoUrl = "http://www.org1Logo.org",
                Location = new Location { Id = 1, Country="USA" },
                Campaigns = new List<Campaign>(),
                Users = new List<ApplicationUser>(),
                OrganizationContacts = new List<OrganizationContact>()
            };
            var campaign = new Campaign
            {
                Id = 1,
                Name = "Campaign 1",
                ManagingOrganizationId = 1
            };
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = "text@example.com"

            };
            var organizationContact = new OrganizationContact
            {
                OrganizationId = 1,
                ContactType = (int)ContactTypes.Primary,
                Contact = new Contact
                {
                    Id = 1,
                    Email = "contact@example.com",
                    FirstName = "firstName",
                    LastName = "lastName",
                    PhoneNumber = "123"
                }
            };

            Context.Organizations.Add(organization);
            Context.Campaigns.Add(campaign);
            Context.Users.Add(user);
            Context.OrganizationContacts.Add(organizationContact);
            Context.SaveChanges();
        }

        [Fact]
        public async Task OrganizationThatMatchesOrganizationIdOnCommand_ReturnsOrganization()
        {
            var query = new OrganizationDetailQuery { Id = 1 };
            var handler = new OrganizationDetailQueryHandler(Context);
            var result = await handler.Handle(query);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task OrganizationsThatDoNotMatchOrganizationIdOnCommand_ReturnsNull()
        {
            var query = new OrganizationDetailQuery { Id = 999};
            var handler = new OrganizationDetailQueryHandler(Context);
            var result = await handler.Handle(query);
            Assert.Null(result);
        }

        [Fact]
        public async Task OrganizationThatMatchesOrganizationIdOnCommand_ReturnsOrganizationDetails()
        {
            var query = new OrganizationDetailQuery { Id = 1 };
            var handler = new OrganizationDetailQueryHandler(Context);
            var result = await handler.Handle(query);
            Assert.Equal(1, result.Id);
            Assert.Equal("http://www.org1.org", result.WebUrl);
            Assert.Equal("http://www.org1Logo.org", result.LogoUrl);
            Assert.Equal("Org 1", result.Name);
            Assert.NotNull(result.Location);
            Assert.Equal(1, result.Location.Id);
            Assert.Equal("USA", result.Location.Country);
        }

        [Fact]
        public async Task OrganizationThatMatchesOrganizationIdOnCommand_ReturnsCampaignDetails()
        {
            var query = new OrganizationDetailQuery { Id = 1 };
            var handler = new OrganizationDetailQueryHandler(Context);
            var result = await handler.Handle(query);
            Assert.Single(result.Campaigns);
            Assert.Equal("Campaign 1", result.Campaigns[0].Name);
        }

        [Fact]
        public async Task OrganizationThatMatchesOrganizationIdOnCommand_ReturnsContactDetails()
        {
            var query = new OrganizationDetailQuery { Id = 1 };
            var handler = new OrganizationDetailQueryHandler(Context);
            var result = await handler.Handle(query);
            Assert.Equal("contact@example.com", result.PrimaryContactEmail);
            Assert.Equal("firstName", result.PrimaryContactFirstName);
            Assert.Equal("lastName", result.PrimaryContactLastName);
            Assert.Equal("123", result.PrimaryContactPhoneNumber);
        }
    }
}
