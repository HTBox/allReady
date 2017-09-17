using AllReady.Areas.Admin.Features.CampaignManagerInvites;
using AllReady.Areas.Admin.Features.Notifications;
using AllReady.Areas.Admin.ViewModels.ManagerInvite;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.CampaignManagerInvites
{
    public class RemoveCampaignManagerInviteCommandHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task ShouldBeAbleToRemoveExistingInvite()
        {
            var handler = new RemoveCampaignManagerInviteCommandHandler(Context);
            Context.CampaignManagerInvites.Add(new CampaignManagerInvite
            {
                Id = 233
            });
            Context.SaveChanges();

            var inviteCommand = new RemoveCampaignManagerInviteCommand
            {
                InviteId = 233
            };
            await handler.Handle(inviteCommand);

            Context.CampaignManagerInvites.Count().ShouldBe(0);
        }

        [Fact]
        public async Task ThrowIfGivenInviteIdDoNotExist()
        {
            var handler = new RemoveCampaignManagerInviteCommandHandler(Context);
            var inviteCommand = new RemoveCampaignManagerInviteCommand
            {
                InviteId = 233
            };

            try
            {
                await handler.Handle(inviteCommand);
            }
            catch (InvalidOperationException ex)
            {
                ex.Message.ShouldContain("Failed to find invite");
            }

        }
    }
}
