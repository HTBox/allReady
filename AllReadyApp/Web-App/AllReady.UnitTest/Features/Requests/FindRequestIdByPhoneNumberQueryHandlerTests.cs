using AllReady.Features.Requests;
using AllReady.Models;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Features.Requests
{
    public class FindRequestIdByPhoneNumberQueryHandlerTests : InMemoryContextTest
    {
        private readonly Guid RequestGuid1 = new Guid("d62723f4-9101-4498-845a-3a46be22aa35");
        private readonly Guid RequestGuid2 = new Guid("d62723f4-9101-4498-845a-3a46be22aa36");
        private readonly Guid RequestGuid3 = new Guid("d62723f4-9101-4498-845a-3a46be22aa37");

        private const string GoodPhoneNumber1 = "0001112222";
        private const string GoodPhoneNumber2 = "0001113333";

        protected override void LoadTestData()
        {
            var request1 = new Request
            {
                RequestId = RequestGuid1,
                Phone = GoodPhoneNumber1,
                DateAdded = new DateTime(2016, 11, 01)
            };

            var request2 = new Request
            {
                RequestId = RequestGuid2,
                Phone = GoodPhoneNumber1,
                DateAdded = new DateTime(2016, 12, 01)
            };

            var request3 = new Request
            {
                RequestId = RequestGuid3,
                Phone = GoodPhoneNumber2,
                DateAdded = new DateTime(2016, 12, 10)
            };

            Context.Requests.Add(request1);
            Context.Requests.Add(request2);
            Context.Requests.Add(request3);

            Context.SaveChanges();
        }

        [Fact]
        public async Task Handle_ReturnEmptyGuid_IfNoMatchingRequest()
        {
            var sut = new FindRequestIdByPhoneNumberQueryHandler(Context);
            var result = await sut.Handle(new FindRequestIdByPhoneNumberQuery("00000"));
            result.ShouldBe(Guid.Empty);
        }

        [Fact]
        public async Task Handle_ReturnCorrectGuid_IfMatchingRequestExists()
        {
            var sut = new FindRequestIdByPhoneNumberQueryHandler(Context);
            var result = await sut.Handle(new FindRequestIdByPhoneNumberQuery(GoodPhoneNumber2));
            result.ShouldBe(RequestGuid3);
        }

        [Fact]
        public async Task Handle_ReturnCorrectGuid_FromMostRecentlyAddedRequest_IfMultipleMatchingRequestsExist()
        {
            var sut = new FindRequestIdByPhoneNumberQueryHandler(Context);
            var result = await sut.Handle(new FindRequestIdByPhoneNumberQuery(GoodPhoneNumber1));
            result.ShouldBe(RequestGuid2);
        }
    }
}
