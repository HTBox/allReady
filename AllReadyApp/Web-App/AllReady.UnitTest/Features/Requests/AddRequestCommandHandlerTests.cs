using System;
using System.Threading.Tasks;
using AllReady.Features.Requests;
using AllReady.Models;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Features.Requests
{
    public class AddRequestCommandHandlerTests : InMemoryContextTest
    {
        private AddRequestCommandHandlerAsync _sut;
        private Mock<IAllReadyDataAccess> _dataAccess;

        public AddRequestCommandHandlerTests()
        {
            _dataAccess = new Mock<IAllReadyDataAccess>();
            _sut = new AddRequestCommandHandlerAsync(_dataAccess.Object);
        }

        [Fact]
        public async Task HandleReturnsNullWhenNoErrorsOccur()
        {
            var command = new AddRequestCommandAsync
            {
                Request = new Request
                {
                    ProviderId = "successId"
                }
            };

            var result = await _sut.Handle(command);

            Assert.Null(result);
        }

        [Fact]
        public async Task WhenNoProviderIdIsProvided_TheStatusIsUnassignedAndIdIsNotNull()
        {
            var request = new Request();
            var command = new AddRequestCommandAsync
            {
                Request = request
            };

            var result = await _sut.Handle(command);

            Assert.NotNull(request.RequestId);
            Assert.Equal(RequestStatus.UnAssigned, request.Status);

        }

        [Fact]
        public async Task WhenProviderIdIsProvidedAndIsValid_TheStatusIsTheSameAsTheRequestAndIdIsTheSame()
        {
            string pid = "someId";
            Guid rid = Guid.NewGuid();
            var request = new Request
            {
                ProviderId = pid,
                Status = RequestStatus.Assigned
            };
            var command = new AddRequestCommandAsync
            {
                Request = request
            };

            Request returnedRequest = new Request
            {
                ProviderId = pid,
                RequestId = rid,
                Status = RequestStatus.UnAssigned
            };
            _dataAccess.Setup(x => x.GetRequestByProviderIdAsync(pid)).ReturnsAsync(returnedRequest);

            await _sut.Handle(command);

            Assert.Equal(rid, returnedRequest.RequestId);
            Assert.Equal(RequestStatus.Assigned, returnedRequest.Status);

        }
    }
}