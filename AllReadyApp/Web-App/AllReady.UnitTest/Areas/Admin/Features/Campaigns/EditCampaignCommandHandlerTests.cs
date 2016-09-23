using AllReady.Areas.Admin.Features.Campaigns;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Campaign;
using AllReady.Areas.Admin.ViewModels.Shared;
using Xunit;
using Moq;
using AllReady.Providers;
using Shouldly;

namespace AllReady.UnitTest.Areas.Admin.Features.Campaigns
{
    public class EditCampaignCommandHandlerTests : InMemoryContextTest
    {
        [Fact]
        public async Task WhenCampaignDoesNotExist_NewCampaignIsAddedToTheDatabase()
        {
            const string name = "Test campaign";

            // arrange
            var vm = new CampaignSummaryViewModel
            {
                Name = name
            };

            var sut = new EditCampaignCommandHandlerAsync(Context, Mock.Of<IConvertDateTimeOffset>());
            var query = new EditCampaignCommandAsync { Campaign = vm };

            // act
            var result = await sut.Handle(query);
            var data = Context.Campaigns.Single(rec => rec.Id == result);

            // assert
            result.ShouldBe(1); // Since no prior records ID should be 1
            data.Name.ShouldBe(name); // The name stored in the DB should match the value passed into the command
        }

        [Fact]
        public async Task UpdatingExistingCampaign_UpdatesLocationWithAllProperties()
        {
            CampaignsHandlerTestHelper.LoadCampaignssHandlerTestData(Context);

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
        public async Task UpdatingExistingCampaign_WithNoPriorContactAddsContactWithAllProperties()
        {
            CampaignsHandlerTestHelper.LoadCampaignssHandlerTestData(Context);

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