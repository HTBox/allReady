using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Requests;
using AllReady.Models;
using AllReady.Models.Notifications;
using AllReady.Services;
using MediatR;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Requests
{
    public class NotifyRequestorsCommandHandlerTests
    {
        [Fact]
        public async Task ShouldSendSmsForEachRequestWithCorrectPhoneNumber()
        {
            var mediator = new Mock<IMediator>();
            var queue = new Mock<IQueueStorageService>();
            var sut = new NotifyRequestorsCommandHandler(queue.Object, mediator.Object);
            string testMessage = "This is a test for ";
            string[] phones =
            {
                "0123456789",
                "1234567890",
                "0987654321"
            };

            var requests = new List<Request>
            {
                new Request {Phone = phones[0]},
                new Request {Phone = phones[1]},
                new Request {Phone = phones[2]}
            };
            var command = new NotifyRequestorsCommand
            {
                NotificationMessageBuilder = ( r, i ) => testMessage + r.Phone,
                Itinerary = new Itinerary
                {
                    Id = 42
                },
                Requests = requests
            };

            await sut.Handle(command);

            var expectedMessages = new List<string>();

            foreach (var request in requests)
            {
                expectedMessages.Add(command.NotificationMessageBuilder(request, command.Itinerary));
            }

            foreach (var expectedMessage in expectedMessages)
            {
                queue.Verify(x => x.SendMessageAsync(QueueStorageService.Queues.SmsQueue, expectedMessage), Times.Once);
            }
        }
    }
}