using AllReady.Attributes;
using AllReady.Controllers;
using AllReady.Features.Requests;
using AllReady.Hangfire.Jobs;
using AllReady.Models;
using AllReady.Services;
using AllReady.UnitTest.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Controllers
{
    public class SmsResponseControllerTests
    {
        private const string GoodPhoneNumber = "0001112222";
        private const string LowercaseConfirm = "y";
        private const string UppercaseConfirm = "Y";
        private const string LowercaseReject = "n";
        private const string UppercaseReject = "N";
        private const string InvalidResponse = "thisisnogood";

        private readonly Guid RequestGuid = new Guid("d62723f4-9101-4498-845a-3a46be22aa35");

        [Fact]
        public void Index_HasHttpPostAttribute()
        {
            var sut = new SmsResponseController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.Index(It.IsAny<string>(), It.IsAny<string>())).OfType<HttpPostAttribute>().SingleOrDefault();
            attribute.ShouldNotBeNull();
        }

        [Fact]
        public void Index_HttpPostAttributeHasCorrectTemplate()
        {
            var sut = new SmsResponseController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.Index(It.IsAny<string>(), It.IsAny<string>())).OfType<HttpPostAttribute>().SingleOrDefault();
            attribute.Template.ShouldBe("smsresponse");
        }

        [Fact]
        public void Index_HasExternalEndpointAttribute()
        {
            var sut = new SmsResponseController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.Index(It.IsAny<string>(), It.IsAny<string>())).OfType<ExternalEndpointAttribute>().SingleOrDefault();
            attribute.ShouldNotBeNull();
        }

        [Fact]
        public void Index_HasAllowAnonymousAttribute()
        {
            var sut = new SmsResponseController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.Index(It.IsAny<string>(), It.IsAny<string>())).OfType<AllowAnonymousAttribute>().SingleOrDefault();
            attribute.ShouldNotBeNull();
        }

        [Fact]
        public async Task Index_ReturnsBadRequestResult_WhenFromIsNull()
        {
            var sut = new SmsResponseController(null, null, null);
            var result = await sut.Index(null, It.IsAny<string>());
            result.ShouldBeOfType(typeof(BadRequestResult));
        }

        [Fact]
        public async Task Index_ReturnsBadRequestResult_WhenFromIsWhitespace()
        {
            var sut = new SmsResponseController(null, null, null);
            var result = await sut.Index(" ", It.IsAny<string>());
            result.ShouldBeOfType(typeof(BadRequestResult));
        }

        [Fact]
        public async Task Index_SendFindRequestIdByPhoneNumberQuery_Once_WhenThePhoneNumberIsNotNullOrWhitespace()
        {
            var mediatr = new Mock<IMediator>();
            mediatr.Setup(x => x.SendAsync(It.IsAny<FindRequestIdByPhoneNumberQuery>()))
                .ReturnsAsync(Guid.Empty)
                .Verifiable();

            var sut = new SmsResponseController(null, mediatr.Object, null);
            var result = await sut.Index(GoodPhoneNumber, It.IsAny<string>());

            mediatr.Verify(x => x.SendAsync(It.IsAny<FindRequestIdByPhoneNumberQuery>()), Times.Once);
        }

        [Fact]
        public async Task Index_SendsFindRequestIdByPhoneNumberQuery_WithCorrectPhoneNumber()
        {
            FindRequestIdByPhoneNumberQuery query = null;

            var mediatr = new Mock<IMediator>();
            mediatr.Setup(x => x.SendAsync(It.IsAny<FindRequestIdByPhoneNumberQuery>()))
                .ReturnsAsync(Guid.Empty)
                .Callback<FindRequestIdByPhoneNumberQuery>(x => query = x)
                .Verifiable();

            var sut = new SmsResponseController(null, mediatr.Object, null);
            var result = await sut.Index(GoodPhoneNumber, It.IsAny<string>());

            query.PhoneNumber.ShouldBe(GoodPhoneNumber);
        }

        [Fact]
        public async Task Index_ReturnsOkResult_WhenRequestIdQueryReturnsEmptyGuid()
        {
            var mediatr = new Mock<IMediator>();
            mediatr.Setup(x => x.SendAsync(It.IsAny<FindRequestIdByPhoneNumberQuery>()))
                .ReturnsAsync(Guid.Empty)
                .Verifiable();

            var sut = new SmsResponseController(null, mediatr.Object, null);
            var result = await sut.Index(GoodPhoneNumber, It.IsAny<string>());

            result.ShouldBeOfType(typeof(OkResult));
        }

        [Fact]
        public async Task Index_CallsChangeRequestStatusTo_WithExpectedValues_WhenUppercaseConfirmBodyPosted()
        {
            Guid requestId = Guid.Empty;
            RequestStatus requestStatus = RequestStatus.PendingConfirmation;

            var mediatr = new Mock<IMediator>();
            mediatr.Setup(x => x.SendAsync(It.IsAny<FindRequestIdByPhoneNumberQuery>()))
                .ReturnsAsync(RequestGuid);

            var changeRequestStatus = new Mock<IChangeRequestStatus>();
            changeRequestStatus.Setup(x => x.To(It.IsAny<RequestStatus>(), It.IsAny<Guid>()))
                .Callback<RequestStatus, Guid>((x, y) => { requestStatus = x; requestId = y; })
                .Verifiable();

            var sut = new SmsResponseController(changeRequestStatus.Object, mediatr.Object, Mock.Of<ISmsSender>());
            await sut.Index(GoodPhoneNumber, UppercaseConfirm);

            changeRequestStatus.Verify(x => x.To(It.IsAny<RequestStatus>(), It.IsAny<Guid>()), Times.Once);
            requestId.ShouldBe(RequestGuid);
            requestStatus.ShouldBe(RequestStatus.Confirmed);
        }

        [Fact]
        public async Task Index_CallsChangeRequestStatusTo_WithExpectedValues_WhenLowercaseConfirmBodyPosted()
        {
            Guid requestId = Guid.Empty;
            RequestStatus requestStatus = RequestStatus.PendingConfirmation;

            var mediatr = new Mock<IMediator>();
            mediatr.Setup(x => x.SendAsync(It.IsAny<FindRequestIdByPhoneNumberQuery>()))
                .ReturnsAsync(RequestGuid);

            var changeRequestStatus = new Mock<IChangeRequestStatus>();
            changeRequestStatus.Setup(x => x.To(It.IsAny<RequestStatus>(), It.IsAny<Guid>()))
                .Callback<RequestStatus, Guid>((x, y) => { requestStatus = x; requestId = y; })
                .Verifiable();

            var sut = new SmsResponseController(changeRequestStatus.Object, mediatr.Object, Mock.Of<ISmsSender>());
            await sut.Index(GoodPhoneNumber, LowercaseConfirm);

            changeRequestStatus.Verify(x => x.To(It.IsAny<RequestStatus>(), It.IsAny<Guid>()), Times.Once);
            requestId.ShouldBe(RequestGuid);
            requestStatus.ShouldBe(RequestStatus.Confirmed);
        }

        [Fact]
        public async Task Index_SendsSmsSendAsync_WithExpectedPhoneNumber_WhenLowercaseConfirmodyPosted()
        {
            string smsTo = null;

            var mediatr = new Mock<IMediator>();
            mediatr.Setup(x => x.SendAsync(It.IsAny<FindRequestIdByPhoneNumberQuery>()))
                .ReturnsAsync(RequestGuid);

            var smsSender = new Mock<ISmsSender>();
            smsSender.Setup(x => x.SendSmsAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult<object>(null))
                .Callback<string, string>((x, y) => smsTo = x)
                .Verifiable();

            var sut = new SmsResponseController(Mock.Of<IChangeRequestStatus>(), mediatr.Object, smsSender.Object);
            await sut.Index(GoodPhoneNumber, LowercaseConfirm);

            smsSender.Verify(x => x.SendSmsAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            smsTo.ShouldBe(GoodPhoneNumber);
        }

        [Fact]
        public async Task Index_SendsSmsSendAsync_WithExpectedPhoneNumber_WhenUppercaseConfirmBodyPosted()
        {
            string smsTo = null;

            var mediatr = new Mock<IMediator>();
            mediatr.Setup(x => x.SendAsync(It.IsAny<FindRequestIdByPhoneNumberQuery>()))
                .ReturnsAsync(RequestGuid);

            var smsSender = new Mock<ISmsSender>();
            smsSender.Setup(x => x.SendSmsAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult<object>(null))
                .Callback<string, string>((x, y) => smsTo = x)
                .Verifiable();

            var sut = new SmsResponseController(Mock.Of<IChangeRequestStatus>(), mediatr.Object, smsSender.Object);
            await sut.Index(GoodPhoneNumber, UppercaseConfirm);

            smsSender.Verify(x => x.SendSmsAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            smsTo.ShouldBe(GoodPhoneNumber);
        }

        [Fact]
        public async Task Index_CallsChangeRequestStatusTo_WithExpectedValues_WhenUppercaseRejectBodyPosted()
        {
            Guid requestId = Guid.Empty;
            RequestStatus requestStatus = RequestStatus.PendingConfirmation;

            var mediatr = new Mock<IMediator>();
            mediatr.Setup(x => x.SendAsync(It.IsAny<FindRequestIdByPhoneNumberQuery>()))
                .ReturnsAsync(RequestGuid);

            var changeRequestStatus = new Mock<IChangeRequestStatus>();
            changeRequestStatus.Setup(x => x.To(It.IsAny<RequestStatus>(), It.IsAny<Guid>()))
                .Callback<RequestStatus, Guid>((x, y) => { requestStatus = x; requestId = y; })
                .Verifiable();

            var sut = new SmsResponseController(changeRequestStatus.Object, mediatr.Object, Mock.Of<ISmsSender>());
            await sut.Index(GoodPhoneNumber, UppercaseReject);

            changeRequestStatus.Verify(x => x.To(It.IsAny<RequestStatus>(), It.IsAny<Guid>()), Times.Once);
            requestId.ShouldBe(RequestGuid);
            requestStatus.ShouldBe(RequestStatus.Unassigned);
        }

        [Fact]
        public async Task Index_CallsChangeRequestStatusTo_WithExpectedValues_WhenLowercaseRejectBodyPosted()
        {
            Guid requestId = Guid.Empty;
            RequestStatus requestStatus = RequestStatus.PendingConfirmation;

            var mediatr = new Mock<IMediator>();
            mediatr.Setup(x => x.SendAsync(It.IsAny<FindRequestIdByPhoneNumberQuery>()))
                .ReturnsAsync(RequestGuid);

            var changeRequestStatus = new Mock<IChangeRequestStatus>();
            changeRequestStatus.Setup(x => x.To(It.IsAny<RequestStatus>(), It.IsAny<Guid>()))
                .Callback<RequestStatus, Guid>((x, y) => { requestStatus = x;  requestId = y; })
                .Verifiable();

            var sut = new SmsResponseController(changeRequestStatus.Object, mediatr.Object, Mock.Of<ISmsSender>());
            await sut.Index(GoodPhoneNumber, LowercaseReject);

            changeRequestStatus.Verify(x => x.To(It.IsAny<RequestStatus>(), It.IsAny<Guid>()), Times.Once);
            requestId.ShouldBe(RequestGuid);
            requestStatus.ShouldBe(RequestStatus.Unassigned);
        }

        [Fact]
        public async Task Index_SendsSmsSendAsync_WithExpectedPhoneNumber_WhenLowercaseRejectBodyPosted()
        {
            string smsTo = null;

            var mediatr = new Mock<IMediator>();
            mediatr.Setup(x => x.SendAsync(It.IsAny<FindRequestIdByPhoneNumberQuery>()))
                .ReturnsAsync(RequestGuid);

            var smsSender = new Mock<ISmsSender>();
            smsSender.Setup(x => x.SendSmsAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult<object>(null))
                .Callback<string, string>((x, y) => smsTo = x)
                .Verifiable();

            var sut = new SmsResponseController(Mock.Of<IChangeRequestStatus>(), mediatr.Object, smsSender.Object);
            await sut.Index(GoodPhoneNumber, LowercaseReject);

            smsSender.Verify(x => x.SendSmsAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            smsTo.ShouldBe(GoodPhoneNumber);
        }

        [Fact]
        public async Task Index_SendsSmsSendAsync_WithExpectedPhoneNumber_WhenUppercaseRejectBodyPosted()
        {
            string smsTo = null;

            var mediatr = new Mock<IMediator>();
            mediatr.Setup(x => x.SendAsync(It.IsAny<FindRequestIdByPhoneNumberQuery>()))
                .ReturnsAsync(RequestGuid);

            var smsSender = new Mock<ISmsSender>();
            smsSender.Setup(x => x.SendSmsAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult<object>(null))
                .Callback<string, string>((x, y) => smsTo = x)
                .Verifiable();

            var sut = new SmsResponseController(Mock.Of<IChangeRequestStatus>(), mediatr.Object, smsSender.Object);
            await sut.Index(GoodPhoneNumber, UppercaseReject);

            smsSender.Verify(x => x.SendSmsAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            smsTo.ShouldBe(GoodPhoneNumber);
        }
        
        [Fact]
        public async Task Index_DoesNotCallChangeRequestStatusTo_WhenBodyDoesNotMatchAcceptedOrRejectedStrings()
        {
            var mediatr = new Mock<IMediator>();
            mediatr.Setup(x => x.SendAsync(It.IsAny<FindRequestIdByPhoneNumberQuery>()))
                .ReturnsAsync(RequestGuid);

            var changeRequestStatus = new Mock<IChangeRequestStatus>();
            changeRequestStatus.Setup(x => x.To(It.IsAny<RequestStatus>(), It.IsAny<Guid>()))
                .Verifiable();

            var sut = new SmsResponseController(changeRequestStatus.Object, mediatr.Object, Mock.Of<ISmsSender>());
            await sut.Index(GoodPhoneNumber, InvalidResponse);

            changeRequestStatus.Verify(x => x.To(It.IsAny<RequestStatus>(), It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task Index_DoesNotSendSmsSendAsync_WhenBodyDoesNotMatchAcceptedOrRejectedStrings()
        {
            var mediatr = new Mock<IMediator>();
            mediatr.Setup(x => x.SendAsync(It.IsAny<FindRequestIdByPhoneNumberQuery>()))
                .ReturnsAsync(RequestGuid);

            var smsSender = new Mock<ISmsSender>();
            smsSender.Setup(x => x.SendSmsAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Verifiable();

            var sut = new SmsResponseController(Mock.Of<IChangeRequestStatus>(), mediatr.Object, smsSender.Object);
            await sut.Index(GoodPhoneNumber, InvalidResponse);

            smsSender.Verify(x => x.SendSmsAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Index_Returns500StatusCode_IfAnExceptionIsThrown()
        {
            var mediatr = new Mock<IMediator>();
            mediatr.Setup(x => x.SendAsync(It.IsAny<FindRequestIdByPhoneNumberQuery>()))
                .ReturnsAsync(RequestGuid);

            var changeRequestStatus = new Mock<IChangeRequestStatus>();
            changeRequestStatus.Setup(x => x.To(It.IsAny<RequestStatus>(), It.IsAny<Guid>()))
               .Throws(new Exception());

            var sut = new SmsResponseController(changeRequestStatus.Object, mediatr.Object, Mock.Of<ISmsSender>());
            var result = await sut.Index(GoodPhoneNumber, UppercaseConfirm) as StatusCodeResult;

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(500);
        }
    }
}
