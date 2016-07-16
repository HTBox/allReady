using System;
using System.Linq;
using AllReady.Areas.Admin.Features.Requests;
using AllReady.Models;
using Microsoft.Data.Entity;
using Shouldly;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Requests
{
    public class RequestStatusChangeCommandHandlerTests : InMemoryContextTest
    {
        private readonly Guid _existingRequestId1 = new Guid("b0efa33c-082f-4287-b641-c96994e96c9e");
        private readonly Guid _existingRequestId2 = new Guid("9e60ede8-229a-4827-b8b7-0b13e22ab435");

        protected override void LoadTestData()
        {
            var req1 = new Request
            {
                RequestId = _existingRequestId1,
                Name = "New Request 1",
                Status = RequestStatus.Assigned
            };

            var req2= new Request
            {
                RequestId = _existingRequestId2,
                Name = "New Request 2",
                Status = RequestStatus.Completed
            };

            Context.Requests.Add(req1);
            Context.Requests.Add(req2);
            Context.SaveChanges();

            // We must do this so that the context change tracker is not already tracking the item when we come to attach it from out subject under test
            Context.Entry(req1).State = EntityState.Detached;
            Context.Entry(req2).State = EntityState.Detached;
        }

        [Fact]
        public async void Handle_WithNewStatusCompletedInMessage_UpdatesRequest()
        {
            var query = new RequestStatusChangeCommand
            {
                RequestId = _existingRequestId1,
                NewStatus = RequestStatus.Completed
            };
            
            var handler = new RequestStatusChangeCommandHandlerAsync(Context);

            var result = await handler.Handle(query);

            var requestToValidate = Context.Requests.First(rec => rec.RequestId == _existingRequestId1);
            
            requestToValidate.Status.ShouldBe(RequestStatus.Completed);
            result.ShouldBeTrue();
        }

        [Fact]
        public async void Handle_WithNewStatusAssignedInMessage_UpdatesRequest()
        {
            var query = new RequestStatusChangeCommand
            {
                RequestId = _existingRequestId2,
                NewStatus = RequestStatus.Assigned
            };

            var handler = new RequestStatusChangeCommandHandlerAsync(Context);

            var result = await handler.Handle(query);

            var requestToValidate = Context.Requests.First(rec => rec.RequestId == _existingRequestId2);

            requestToValidate.Status.ShouldBe(RequestStatus.Assigned);
            result.ShouldBeTrue();
        }
    }
}
