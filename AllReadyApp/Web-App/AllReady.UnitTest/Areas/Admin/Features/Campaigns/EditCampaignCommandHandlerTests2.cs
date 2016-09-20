using AllReady.Areas.Admin.Features.Campaigns;
using System;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Campaign;
using AllReady.Areas.Admin.ViewModels.Shared;
using AllReady.Providers;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Campaigns
{
    //TODO: move these tests into EditCampaignCommandHandlerTests1, then delete this class
    public class EditCampaignCommandHandlerTests2 : InMemoryContextTest
    {
        protected override void LoadTestData()
        {
            CampaignsHandlerTestHelper.LoadCampaignssHandlerTestData(Context);
        }

        [Fact]
        public async Task AddNewCampaign()
        {
            // Arrange
            var handler = new EditCampaignCommandHandlerAsync(Context, Mock.Of<IConvertDateTimeOffset>());
            var newCampaign = new CampaignSummaryViewModel { Name = "New", Description = "Desc", TimeZoneId = "UTC" };

            // Act
            var result = await handler.Handle(new EditCampaignCommandAsync { Campaign = newCampaign });

            // Assert
            Assert.Equal(5, Context.Campaigns.Count());
            Assert.True(result > 0);
        }

        /// <summary>
        /// Tests that the columms belonging the campaign table record are actually updated when a campaign is edited
        /// </summary>
        /// <remarks>This test is not testing the creation of location record, or impact record as those should be seperate tests</remarks>
        [Fact]
        public async Task UpdatingExistingCampaignUpdatesAllCoreProperties()
        {
            // Arrange
            const string name = "New Name";
            const string desc = "New Desc";
            const string fullDesc = "New Full Desc";
            const string timezoneId = "GMT Standard Time";
            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(30);
            const int org = 2;

            var handler = new EditCampaignCommandHandlerAsync(Context, Mock.Of<IConvertDateTimeOffset>());
            var updatedCampaign = new CampaignSummaryViewModel
            {
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
            var result = await handler.Handle(new EditCampaignCommandAsync { Campaign = updatedCampaign });
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

        [Fact]
        public async Task UpdatingExistingCampaignUpdatesLocationWithAllProperties()
        {
            // Arrange
            const string name = "New Name";
            const string desc = "New Desc";
            const int org = 2;
            const string address1 = "Address 1";
            const string address2 = "Address 1";
            const string city = "City";
            const string state = "State";
            const string postcode = "45231";
            const string country = "USA";

            var handler = new EditCampaignCommandHandlerAsync(Context, Mock.Of<IConvertDateTimeOffset>());
            var updatedCampaign = new CampaignSummaryViewModel
            {
                Id = 2,
                Name = name,
                Description = desc,
                OrganizationId = org,
                TimeZoneId = "GMT Standard Time",
                Location = new LocationEditViewModel { Address1 = address1, Address2 = address2, City = city, State = state, PostalCode = postcode }
            };

            // Act
            await handler.Handle(new EditCampaignCommandAsync { Campaign = updatedCampaign });
            var savedCampaign = Context.Campaigns.SingleOrDefault(s => s.Id == 2);

            // Assert
            Assert.Equal(address1, savedCampaign.Location.Address1);
            Assert.Equal(address2, savedCampaign.Location.Address2);
            Assert.Equal(city, savedCampaign.Location.City);
            Assert.Equal(state, savedCampaign.Location.State);
            Assert.Equal(postcode, savedCampaign.Location.PostalCode);
            Assert.Equal(country, savedCampaign.Location.Country);
        }

        [Fact]
        public async Task UpdatingExistingCampaignWithNoPriorContactAddsContactWithAllProperties()
        {
            // Arrange
            const string name = "New Name";
            const string desc = "New Desc";
            const int org = 2;
            const string contactEmail = "jdoe@example.com";
            const string firstname = "John";
            const string lastname = "Doe";
            const string telephone = "01323 111111";

            var handler = new EditCampaignCommandHandlerAsync(Context, Mock.Of<IConvertDateTimeOffset>());
            var updatedCampaign = new CampaignSummaryViewModel
            {
                Id = 2,
                Name = name,
                Description = desc,
                OrganizationId = org,
                TimeZoneId = "GMT Standard Time",
                PrimaryContactEmail = contactEmail,
                PrimaryContactFirstName = firstname,
                PrimaryContactLastName = lastname,
                PrimaryContactPhoneNumber = telephone
            };

            // Act
            await handler.Handle(new EditCampaignCommandAsync { Campaign = updatedCampaign });
            var newContact = Context.Contacts.OrderBy(c => c.Id).LastOrDefault();

            // Assert
            Assert.Equal(2, Context.CampaignContacts.Count());
            Assert.Equal(2, Context.Contacts.Count());

            Assert.NotNull(newContact);

            Assert.Equal(contactEmail, newContact.Email);
            Assert.Equal(firstname, newContact.FirstName);
            Assert.Equal(lastname, newContact.LastName);
            Assert.Equal(telephone, newContact.PhoneNumber);
        }
    }
}