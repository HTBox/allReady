using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Moq;
using Xunit;
using Shouldly;
using AllReady.Areas.Admin.Features.Invite;
using AllReady.Areas.Admin.ViewModels.Invite;
using System.Threading.Tasks;

namespace AllReady.UnitTest.Areas.Admin.Features.Invite
{
    public class CreateInviteCommandHandlerShould : InMemoryContextTest
    {
        protected override void LoadTestData()
        {
            Context.Campaigns.Add(new AllReady.Models.Campaign
            {
                Id = 1
            });

            Context.Events.Add(new AllReady.Models.Event
            {
                Id = 5,
                CampaignId = 1
            });

            Context.SaveChanges();
        }

        [Fact]
        public void ThrowArgumentException_WhenMessageIsNUll()
        {
            var handler = new CreateInviteCommandHandler(Context);
            Should.Throw<ArgumentException>(async () => await handler.Handle(null));
        }

        [Fact]
        public void ThrowArgumentException_WhenInviteIsNull()
        {
            var handler = new CreateInviteCommandHandler(Context);
            var inviteCommand = new CreateInviteCommand
            {
                Invite = null,
                UserId = "userId"
            };

            Should.Throw<ArgumentException>(async () => await handler.Handle(inviteCommand));
        }

        [Fact]
        public void ThowArgumentException_WhenUserIdIsNull_AndInviteTypeIsCampaignInvite()
        {
            var handler = new CreateInviteCommandHandler(Context);
            var invite = CreateValidInviteViewModelForCampaignManagerInvite();

            var inviteCommand = new CreateInviteCommand
            {
                Invite = invite,
                UserId = null
            };

            Should.Throw<ArgumentException>(async () => await handler.Handle(inviteCommand));
        }

        [Fact]
        public void ThowArgumentException_WhenUserIdIsNull_AndInviteTypeIsEventInvite()
        {
            var handler = new CreateInviteCommandHandler(Context);
            var invite = CreateValidInviteViewModelForEventManagerInvite();

            var inviteCommand = new CreateInviteCommand
            {
                Invite = invite,
                UserId = null
            };

            Should.Throw<ArgumentException>(async () => await handler.Handle(inviteCommand));
        }

        [Fact]
        public void ThrowArgumentException_WhenInviteeEmailIsNull_AndInviteTypeIsEventManager()
        {
            var handler = new CreateInviteCommandHandler(Context);
            var invite = CreateValidInviteViewModelForEventManagerInvite();
            invite.InviteeEmailAddress = null;

            var inviteCommand = new CreateInviteCommand
            {
                Invite = invite,
                UserId = "userId"
            };

            Should.Throw<ArgumentException>(async () => await handler.Handle(inviteCommand));
        }

        [Fact]
        public void ThrowArgumentException_WhenInviteeEmailIsNull_AndInviteTypeIsCampaignManager()
        {
            var handler = new CreateInviteCommandHandler(Context);
            var invite = CreateValidInviteViewModelForCampaignManagerInvite();
            invite.InviteeEmailAddress = null;

            var inviteCommand = new CreateInviteCommand
            {
                Invite = invite,
                UserId = "userId"
            };

            Should.Throw<ArgumentException>(async () => await handler.Handle(inviteCommand));
        }

        [Fact]
        public void ThrowArgumentException_WhenInviteeEmailIsBlank_AndInviteTypeIsEventManager()
        {
            var handler = new CreateInviteCommandHandler(Context);
            var invite = CreateValidInviteViewModelForEventManagerInvite();
            invite.InviteeEmailAddress = string.Empty;

            var inviteCommand = new CreateInviteCommand
            {
                Invite = invite,
                UserId = "userId"
            };

            Should.Throw<ArgumentException>(async () => await handler.Handle(inviteCommand));
        }

        [Fact]
        public void ThrowArgumentException_WhenInviteeEmailIsBlank_AndInviteTypeIsCampaignManager()
        {
            var handler = new CreateInviteCommandHandler(Context);
            var invite = CreateValidInviteViewModelForCampaignManagerInvite();
            invite.InviteeEmailAddress = string.Empty;

            var inviteCommand = new CreateInviteCommand
            {
                Invite = invite,
                UserId = "userId"
            };

            Should.Throw<ArgumentException>(async () => await handler.Handle(inviteCommand));
        }

        [Fact]
        public void ThrowArgumentException_WhenEventIdIsNotValid_AndInviteTypeIsEventManager()
        {
            var handler = new CreateInviteCommandHandler(Context);
            var invite = new InviteViewModel
            {
                InviteType = InviteType.EventManagerInvite,
                EventId = 2,
            };

            var inviteCommand = new CreateInviteCommand
            {
                Invite = invite,
                UserId = "userId"
            };

            Should.Throw<ArgumentException>(async () => await handler.Handle(inviteCommand));
        }

        [Fact]
        public void ThrowArgumentException_WhenCampaignIdIsNotValid_AndInviteTypeIsCampaignManager()
        {
            var handler = new CreateInviteCommandHandler(Context);
            var invite = new InviteViewModel
            {
                InviteType = InviteType.CampaignManagerInvite,
                CampaignId = 10,
            };

            var inviteCommand = new CreateInviteCommand
            {
                Invite = invite,
                UserId = "userId"
            };

            Should.Throw<ArgumentException>(async () => await handler.Handle(inviteCommand));
        }

        [Fact]
        public async Task CreateEventManagerInvite_WhenInviteTypeIsEventManager()
        {
            var handler = new CreateInviteCommandHandler(Context);
            var invite = new InviteViewModel
            {
                CustomMessage = "message",
                EventId = 5,
                InviteeEmailAddress = "test@test.com",
                InviteType = InviteType.EventManagerInvite,
            };

            var inviteCommand = new CreateInviteCommand
            {
                Invite = invite,
                UserId = "userId"
            };

            handler.DateTimeUtcNow = () => new DateTime(2016, 5, 29);

            await handler.Handle(inviteCommand);

            Context.CampaignManagerInvites.Count().ShouldBe(0);
            Context.EventManagerInvites.Count().ShouldBe(1);
            Context.EventManagerInvites.FirstOrDefault().AcceptedDateTimeUtc.ShouldBe(null);
            Context.EventManagerInvites.FirstOrDefault().CustomMessage.ShouldBe("message");
            Context.EventManagerInvites.FirstOrDefault().EventId.ShouldBe(5);
            Context.EventManagerInvites.FirstOrDefault().InviteeEmailAddress.ShouldBe("test@test.com");
            Context.EventManagerInvites.FirstOrDefault().RejectedDateTimeUtc.ShouldBe(null);
            Context.EventManagerInvites.FirstOrDefault().RevokedDateTimeUtc.ShouldBe(null);
            Context.EventManagerInvites.FirstOrDefault().SenderUserId.ShouldBe("userId");
            Context.EventManagerInvites.FirstOrDefault().SentDateTimeUtc.ShouldBe(new DateTime(2016, 5, 29));
        }

        [Fact]
        public async Task CreateCampaignManagerInvite_WhenInviteTypeIsCampaignManager()
        {
            var handler = new CreateInviteCommandHandler(Context);
            var invite = new InviteViewModel
            {
                CustomMessage = "message",
                CampaignId = 1,
                InviteeEmailAddress = "test@test.com",
                InviteType = InviteType.CampaignManagerInvite,
            };

            var inviteCommand = new CreateInviteCommand
            {
                Invite = invite,
                UserId = "userId"
            };

            handler.DateTimeUtcNow = () => new DateTime(2016, 5, 29);

            await handler.Handle(inviteCommand);

            Context.EventManagerInvites.Count().ShouldBe(0);
            Context.CampaignManagerInvites.Count().ShouldBe(1);
            Context.CampaignManagerInvites.FirstOrDefault().AcceptedDateTimeUtc.ShouldBe(null);
            Context.CampaignManagerInvites.FirstOrDefault().CustomMessage.ShouldBe("message");
            Context.CampaignManagerInvites.FirstOrDefault().CampaignId.ShouldBe(1);
            Context.CampaignManagerInvites.FirstOrDefault().InviteeEmailAddress.ShouldBe("test@test.com");
            Context.CampaignManagerInvites.FirstOrDefault().RejectedDateTimeUtc.ShouldBe(null);
            Context.CampaignManagerInvites.FirstOrDefault().RevokedDateTimeUtc.ShouldBe(null);
            Context.CampaignManagerInvites.FirstOrDefault().SenderUserId.ShouldBe("userId");
            Context.CampaignManagerInvites.FirstOrDefault().SentDateTimeUtc.ShouldBe(new DateTime(2016, 5, 29));
        }

        private InviteViewModel CreateValidInviteViewModelForCampaignManagerInvite()
        {
            return new InviteViewModel
            {
                CampaignId = 1,
                CustomMessage = "message",
                InviteeEmailAddress = "test@test.com",
                InviteType = InviteType.CampaignManagerInvite,
            };
        }

        private InviteViewModel CreateValidInviteViewModelForEventManagerInvite()
        {
            return new InviteViewModel
            {
                EventId = 5,
                CustomMessage = "message",
                InviteeEmailAddress = "test@test.com",
                InviteType = InviteType.EventManagerInvite,
            };
        }
    }
}
