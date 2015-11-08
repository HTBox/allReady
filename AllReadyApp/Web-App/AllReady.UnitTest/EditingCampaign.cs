using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Areas.Admin.ViewModels;
using Xunit;

namespace AllReady.UnitTests
{
    public class EditingCampaign : InMemoryContextTest
    {
        [Fact]
        public void ModelIsCreated()
        {
            var sut = new EditCampaignCommandHandler(Context);
            int actual = sut.Handle(new EditCampaignCommand {Campaign = new CampaignSummaryViewModel()});
            Assert.Equal(1, actual);
        }
    }
}