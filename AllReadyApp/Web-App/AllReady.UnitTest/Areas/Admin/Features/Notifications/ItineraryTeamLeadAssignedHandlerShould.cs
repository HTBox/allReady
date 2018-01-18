using System.Text;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Notifications;
using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Notifications
{
    public class ItineraryTeamLeadAssignedHandlerShould
    {
        private const string itineraryName = "The itinerary";
        private const string assigneeEmail = "test@test.com";
        private const string assigneePhone = "+1 (999) 999-9999";
        private const string itineraryUrl = "https://example.com";
        private const string subject = @"You have been assigned as the Team Lead on an itinerary";
        private string smsMessage = $@"You have been assigned as the Team Lead on the following itenerary: ""{itineraryName}"".";

        [Fact]
        public async Task SendTeamLeadAssignmentMessageToVolunteer()
        {
            var mockMediator = new Mock<IMediator>();
            var handler = new IterneraryTeamLeadAssignedHandler(mockMediator.Object);

            await handler.Handle(new IteneraryTeamLeadAssigned()
            {
                AssigneePhone = assigneePhone,
                AssigneeEmail = assigneeEmail,
                ItineraryName = itineraryName,
                ItineraryUrl = itineraryUrl
            });

            var plaintextMsg = new StringBuilder();
            plaintextMsg.AppendLine($@"You have been assigned as the Team Lead on the following itenerary: ""{itineraryName}"".");
            plaintextMsg.AppendLine();
            plaintextMsg.AppendLine($"To view the itinerary go to the following Url: \"{itineraryUrl}\".");
            var plainTextMessage = plaintextMsg.ToString();
            var htmlMsg = new StringBuilder();
            htmlMsg.AppendLine($@"You have been assigned as the Team Lead on the following itenerary: ""{itineraryName}"".");
            htmlMsg.AppendLine();
            htmlMsg.AppendLine($"To view the itinerary <a href=\"{itineraryUrl}\">click here</a>.");
            var htmlMessage = htmlMsg.ToString();

            mockMediator.Verify(x => x.SendAsync(It.Is<NotifyVolunteersCommand>(cmd =>
                cmd.ViewModel != null &&
                cmd.ViewModel.Subject == subject &&
                cmd.ViewModel.EmailMessage == plainTextMessage &&
                cmd.ViewModel.HtmlMessage == htmlMessage &&
                cmd.ViewModel.SmsMessage == smsMessage &&
                cmd.ViewModel.SmsRecipients.Contains(assigneePhone) &&
                cmd.ViewModel.EmailRecipients.Contains(assigneeEmail))), Times.Once());
        }

    }
}
