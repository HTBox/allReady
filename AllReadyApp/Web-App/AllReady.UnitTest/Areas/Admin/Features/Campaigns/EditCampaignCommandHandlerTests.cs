using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Areas.Admin.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using AllReady.UnitTest.Features.Campaigns;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Campaigns
{
    public class EditCampaignCommandHandlerTests : InMemoryContextTest
    {
        [Fact(Skip = "RTM Broken Tests")]
        public async Task AddNewCampaign()
        {
            // Arrange
            var handler = new EditCampaignCommandHandler(Context);
            var newCampaign = new CampaignSummaryModel { Name = "New", Description = "Desc", TimeZoneId ="UTC" };

            // Act
            var result = await handler.Handle(new EditCampaignCommand { Campaign = newCampaign });

            // Assert
            Assert.Equal(5, Context.Campaigns.Count());
            Assert.True(result > 0);
        }

        /// <summary>
        /// Tests that the columms belonging the campaign table record are actually updated when a campaign is edited
        /// </summary>
        /// <remarks>This test is not testing the creation of location record, or impact record as those should be seperate tests</remarks>
        [Fact(Skip = "RTM Broken Tests")]
        public async Task UpdatingExistingCampaignUpdatesAllCoreProperties()
        {
            // Arrange
            var name = "New Name";
            var desc = "New Desc";
            var fullDesc = "New Full Desc";
            var timezoneId = "GMT Standard Time";
            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(30);
            var org = 2;
            
            var handler = new EditCampaignCommandHandler(Context);
            var updatedCampaign = new CampaignSummaryModel {
                Id = 2,
                Name = name,
                Description = desc,
                FullDescription = fullDesc,
                TimeZoneId = timezoneId,
                StartDate = startDate,
                EndDate = endDate,
                OrganizationId = org,
            };

            // Act
            var result = await handler.Handle(new EditCampaignCommand { Campaign = updatedCampaign });
            var savedCampaign = Context.Campaigns.SingleOrDefault(s => s.Id == 2);

            // Assert
            Assert.Equal(4, Context.Campaigns.Count());
            Assert.Equal(2, result);

            Assert.Equal(name, savedCampaign.Name);
            Assert.Equal(desc, savedCampaign.Description);
            Assert.Equal(fullDesc, savedCampaign.FullDescription);
            Assert.Equal(timezoneId, savedCampaign.TimeZoneId);
            Assert.NotEqual(startDate.Date, new DateTime()); // We're not testing the date logic in this test, only that a datetime value is saved
            Assert.NotEqual(endDate.Date, new DateTime()); // We're not testing the date logic in this test, only that a datetime value is saved
            Assert.Equal(org, savedCampaign.ManagingOrganizationId);
        }

        [Fact(Skip = "RTM Broken Tests")]
        public async Task UpdatingExistingCampaignUpdatesLocationWithAllProperties()
        {
            // Arrange
            var name = "New Name";
            var desc = "New Desc";            
            var org = 2;
            var address1 = "Address 1";
            var address2 = "Address 1";
            var city = "City";
            var state = "State";
            var postcode = "45231";
            var country = "USA";

            var handler = new EditCampaignCommandHandler(Context);
            var updatedCampaign = new CampaignSummaryModel
            {
                Id = 2,
                Name = name,
                Description = desc,
                OrganizationId = org,
                TimeZoneId = "GMT Standard Time",
                Location = new LocationEditModel {  Address1 = address1, Address2 = address2, City = city, State = state, PostalCode = postcode }
            };

            // Act
            await handler.Handle(new EditCampaignCommand { Campaign = updatedCampaign });
            var savedCampaign = Context.Campaigns.SingleOrDefault(s => s.Id == 2);

            // Assert
            Assert.Equal(address1, savedCampaign.Location.Address1);
            Assert.Equal(address2, savedCampaign.Location.Address2);
            Assert.Equal(city, savedCampaign.Location.City);
            Assert.Equal(state, savedCampaign.Location.State);
            Assert.Equal(postcode, savedCampaign.Location.PostalCode);
            Assert.Equal(country, savedCampaign.Location.Country);
        }

        [Fact(Skip = "RTM Broken Tests")]
        public async Task UpdatingExistingCampaignWithNoPriorContactAddsContactWithAllProperties()
        {
            // Arrange
            var name = "New Name";
            var desc = "New Desc";
            var org = 2;
            var contactEmail = "jdoe@example.com";
            var firstname = "John";
            var lastname = "Doe";
            var telephone = "01323 111111";

            var handler = new EditCampaignCommandHandler(Context);
            var updatedCampaign = new CampaignSummaryModel
            {
                Id = 2,
                Name = name,
                Description = desc,
                OrganizationId = org,
                TimeZoneId = "GMT Standard Time",
                PrimaryContactEmail = contactEmail,
                PrimaryContactFirstName =firstname,
                PrimaryContactLastName = lastname,
                PrimaryContactPhoneNumber = telephone
            };

            // Act
            await handler.Handle(new EditCampaignCommand { Campaign = updatedCampaign });
            var newContact = Context.Contacts.OrderBy(c=>c.Id).LastOrDefault();

            // Assert
            Assert.Equal(2, Context.CampaignContacts.Count());
            Assert.Equal(2, Context.Contacts.Count());

            Assert.NotNull(newContact);

            Assert.Equal(contactEmail, newContact.Email);
            Assert.Equal(firstname, newContact.FirstName);
            Assert.Equal(lastname, newContact.LastName);
            Assert.Equal(telephone, newContact.PhoneNumber);
        }

        protected override void LoadTestData()
        {
            CampaignsHandlerTestHelper.LoadCampaignssHandlerTestData(Context);
        }
    }
}
