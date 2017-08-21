using AllReady.Models;

namespace AllReady.UnitTest.Features.Organizations
{
    public static class OrganizationHandlerTestHelper
    {
        public static void LoadOrganizationHandlerTestData(AllReadyContext context)
        {
            const string htmlPrivacyPolicy = "<h2>Line 1</h2><p>Line 2</p>";

            var org1 = new Organization { Id = 1, Name = "Org 1", Location = new Location() };
            var org2 = new Organization
            {
                Id = 2,
                Name = "Org 2",
                PrivacyPolicy = htmlPrivacyPolicy,
                Location = new Location()
            };

            // Organizations
            context.Organizations.Add(org1);
            context.Organizations.Add(org2);

            // Campaigns
            context.Campaigns.Add(new Campaign { Name = "Campaign 1", ManagingOrganization = org1, Location = new Location() });
            context.Campaigns.Add(new Campaign { Name = "Campaign 2", ManagingOrganization = org1, Location = new Location() });
            context.Campaigns.Add(new Campaign { Name = "Locked Campaign", Locked = true, ManagingOrganization = org1, Location = new Location() });
            context.Campaigns.Add(new Campaign { Name = "Unlocked Campaign", Locked = false, ManagingOrganization = org1, Location = new Location() });

            context.SaveChanges();
        }
    }
}
