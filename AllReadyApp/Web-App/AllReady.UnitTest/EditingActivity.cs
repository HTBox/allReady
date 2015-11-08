using AllReady.Areas.Admin.Features.Activities;
using AllReady.Areas.Admin.ViewModels;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTests
{
    public class EditingActivity : InMemoryContextTest
    {
        public EditingActivity()
        {
            // Adding an activity requires a campaign for a tenant ID and an activity to match that in the command
            Context.Campaigns.Add(new Campaign {Id = 1});
            Context.Activities.Add(new Activity {Id = 1});
            Context.SaveChanges();
        }

        [Fact]
        public void ModelIsCreated()
        {
            var sut = new EditActivityCommandHandler(Context);
            int actual =
                sut.Handle(new EditActivityCommand {Activity = new ActivityDetailViewModel {CampaignId = 1, Id = 1}});
            Assert.Equal(1, actual);
        }
    }
}