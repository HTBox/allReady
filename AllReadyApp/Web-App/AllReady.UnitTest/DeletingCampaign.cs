using System.Linq;
using AllReady.Areas.Admin.Features.Campaigns;
using Xunit;

namespace AllReady.UnitTests
{
    public class DeletingCampaign : InMemoryContextTest
    {
        private const int CampaignId = 1;

        [Fact]
        public void CampaignIsDeleted()
        {
            var sut = new DeleteCampaignCommandHandler(Context);
            sut.Handle(new DeleteCampaignCommand { CampaignId = CampaignId });
            Assert.False(Context.Activities.Any(t => t.Id == CampaignId));
        }

        [Fact]
        public void NonExistantTaskDoesNotCauseException()
        {
            var sut = new DeleteCampaignCommandHandler(Context);
            sut.Handle(new DeleteCampaignCommand { CampaignId = 666 });
            Assert.False(Context.Activities.Any(t => t.Id == CampaignId));
        }
    }
}