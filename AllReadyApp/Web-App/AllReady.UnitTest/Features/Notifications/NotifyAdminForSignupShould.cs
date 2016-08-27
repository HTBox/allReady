using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using AllReady.Features.Notifications;
using MediatR;

namespace AllReady.UnitTest.Features.Notifications
{
    public class NotifyAdminForSignupShould : InMemoryContextTest
    {
        [Fact(Skip = "NotImplemented")]
        public void PassANotifyVolunteersCommandToTheMediator()
        {
            // TODO: Implement test
        }

        [Fact(Skip = "NotImplemented")]
        public void SendToTheAdminEmail()
        {
            // TODO: Implement test
        }

        [Fact(Skip = "NotImplemented")]
        public void LogIfAnExceptionOccurs()
        {
            // TODO: Implement test
        }

        [Fact(Skip = "NotImplemented")]
        public void SkipNotificationIfAdminEmailIsNotSpecified()
        {
            // TODO: Implement test
        }


    }
}