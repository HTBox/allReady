using AllReady.Areas.Admin.Features.Notifications;
using AllReady.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.CampaignManagerInvites
{
    public class CreateCampaignManagerInviteCommandHandler : AsyncRequestHandler<CreateCampaignManagerInviteCommand>
    {
        private readonly AllReadyContext _context;
        private readonly IMediator _mediator;
        private readonly IUrlHelper _urlHelper;

        public CreateCampaignManagerInviteCommandHandler(AllReadyContext context, IMediator mediator, IUrlHelper urlHelper)
        {
            _context = context;
            _mediator = mediator;
            _urlHelper = urlHelper;
        }

        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        protected override async Task HandleCore(CreateCampaignManagerInviteCommand message)
        {
            var campaignManagerInvite = new CampaignManagerInvite
            {
                InviteeEmailAddress = message.Invite.InviteeEmailAddress,
                SentDateTimeUtc = DateTimeUtcNow(),
                CustomMessage = message.Invite.CustomMessage,
                SenderUserId = message.UserId,
                CampaignId = message.Invite.CampaignId
            };
            _context.CampaignManagerInvites.Add(campaignManagerInvite);
            await _context.SaveChangesAsync();

            await _mediator.PublishAsync(new CampaignManagerInvited
            {
                InviteeEmail = message.Invite.InviteeEmailAddress,
                CampaignName = message.Invite.CampaignName,
                SenderName = message.SenderName,
                AcceptUrl = _urlHelper.Link("CampaignManagerInviteAcceptRoute", new { inviteId = campaignManagerInvite.Id }),
                DeclineUrl = _urlHelper.Link("CampaignManagerInviteDeclineRoute", new { inviteId = campaignManagerInvite.Id }),
                RegisterUrl = message.RegisterUrl,
                IsInviteeRegistered = message.IsInviteeRegistered,
                Message = message.Invite.CustomMessage
            });
        }
    }
}
