using AllReady.Areas.Admin.Features.Activities;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using AllReady.UnitTest.Features.Campaigns;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Activities
{
    public class EditingActivity : InMemoryContextTest
    {
        public EditingActivity()
        {
            // Adding an activity requires a campaign for a organization ID and an activity to match that in the command
            Context.Campaigns.Add(new Campaign {Id = 1, TimeZoneId = "Central Standard Time" });
            Context.Activities.Add(new Activity {Id = 1});
            Context.SaveChanges();
        }

        [Fact]
        public void ModelIsCreated()
        {
            var sut = new EditActivityCommandHandler(Context);
            var actual = sut.Handle(new EditActivityCommand {Activity = new ActivityDetailModel {CampaignId = 1, Id = 1, TimeZoneId = "Central Standard Time"}});
            Assert.Equal(1, actual);
        }
    }
}