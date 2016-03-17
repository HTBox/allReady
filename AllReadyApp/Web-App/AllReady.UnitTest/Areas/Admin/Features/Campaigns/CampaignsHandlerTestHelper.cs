using AllReady.Models;

namespace AllReady.UnitTest.Areas.Admin.Features.Campaigns
{
    public static class CampaignsHandlerTestHelper
    {
        public static void LoadCampaignssHandlerTestData(AllReadyContext context)
        {
            // Organizations
            context.Organizations.Add(new Organization { Id = 1, Name = "Org 1" });
            context.Organizations.Add(new Organization { Id = 2, Name = "Org 2" });

            // Campaigns
            context.Campaigns.Add(new Campaign { Name = "Campaign 1", ManagingOrganizationId = 1 });
            context.Campaigns.Add(new Campaign { Name = "Campaign 2", ManagingOrganizationId = 1 });
            context.Campaigns.Add(new Campaign { Name = "Locked Campaign", ManagingOrganizationId = 1, Locked = true });
            context.Campaigns.Add(new Campaign { Name = "Unlocked Campaign", ManagingOrganizationId = 1, Locked = false });

            // Contacts

            var newContact = new Contact { Email = "something@example.com", FirstName = "Bill", LastName = "Gates", PhoneNumber = "01323 000000" };
            context.Contacts.Add(newContact);
            context.CampaignContacts.Add(new CampaignContact { CampaignId = 1, ContactId = newContact.Id });
            
            // Geo
            context.PostalCodes.Add(new PostalCodeGeo { City = "Cincinnati", PostalCode = "45231", State = "OH" });

            context.SaveChanges();
        }
    }
}