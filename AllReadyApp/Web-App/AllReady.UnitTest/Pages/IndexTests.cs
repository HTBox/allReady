using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AllReady.Features.Campaigns;
using AllReady.Features.Home;
using AllReady.Pages;
using MediatR;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Pages
{
    public class IndexTests
    {
        [Fact]
        public async Task IndexSendsActiveOrUpcomingEventsQuery()
        {
            var mockMediator = new Mock<IMediator>();

            var sut = new IndexModel(mockMediator.Object)
            {
                TempData = Mock.Of<ITempDataDictionary>()
            };
            await sut.OnGetAsync();

            mockMediator.Verify(x => x.SendAsync(It.IsAny<ActiveOrUpcomingEventsQuery>()), Times.Once());
        }

        [Fact]
        public async Task IndexSendsFeaturedCampaignQuery()
        {
            var mockMediator = new Mock<IMediator>();

            var sut = new IndexModel(mockMediator.Object)
            {
                TempData = Mock.Of<ITempDataDictionary>()
            };
            await sut.OnGetAsync();

            mockMediator.Verify(x => x.SendAsync(It.IsAny<FeaturedCampaignQuery>()), Times.Once());
        }
    }
}
