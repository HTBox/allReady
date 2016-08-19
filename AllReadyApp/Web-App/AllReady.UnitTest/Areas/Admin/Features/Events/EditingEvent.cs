using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Events;
using AllReady.Areas.Admin.ViewModels.Event;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Events
{
    public class EditingEvent : InMemoryContextTest
    {
        //public EditingEvent()
        //{
        //    // Adding an event requires a campaign for a organization ID and an event to match that in the command
        //    Context.Campaigns.Add(new Campaign {Id = 1, TimeZoneId = "Central Standard Time" });
        //    Context.Events.Add(new Event {Id = 1});
        //    Context.SaveChanges();
        //}

        //[Fact]
        //public async Task ModelIsCreated()
        //{
        //    var sut = new EditEventCommandHandler(Context);
        //    var actual = await sut.Handle(new EditEventCommand {Event = new EventEditViewModel { CampaignId = 1, Id = 1, TimeZoneId = "Central Standard Time"}});
        //    Assert.Equal(1, actual);
        //}
    }
}