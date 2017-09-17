using AllReady.Areas.Admin.Features.Notifications;
using AllReady.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.EventManagerInvites
{
    public class CreateEventManagerInviteCommandHandler : AsyncRequestHandler<CreateEventManagerInviteCommand>
    {
        private readonly AllReadyContext _context;
        private readonly IMediator _mediator;
        private readonly IUrlHelper _urlHelper;

        public CreateEventManagerInviteCommandHandler(AllReadyContext context, IMediator mediator, IUrlHelper urlHelper)
        {
            _context = context;
            _mediator = mediator;
            _urlHelper = urlHelper;
        }

        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        protected override async Task HandleCore(CreateEventManagerInviteCommand message)
        {
            var eventManagerInvite = new EventManagerInvite
            {
                InviteeEmailAddress = message.Invite.InviteeEmailAddress,
                SentDateTimeUtc = DateTimeUtcNow(),
                CustomMessage = message.Invite.CustomMessage,
                SenderUserId = message.UserId,
                EventId = message.Invite.EventId
            };
            _context.EventManagerInvites.Add(eventManagerInvite);
            await _context.SaveChangesAsync();

            await _mediator.PublishAsync(new EventManagerInvited
            {
                InviteeEmail = message.Invite.InviteeEmailAddress,
                EventName = message.Invite.EventName,
                SenderName = message.SenderName,
                AcceptUrl = _urlHelper.Link("EventManagerInviteAcceptRoute", new { inviteId = eventManagerInvite.Id }),
                DeclineUrl = _urlHelper.Link("EventManagerInviteDeclineRoute", new { inviteId = eventManagerInvite.Id }),
                RegisterUrl = message.RegisterUrl,
                IsInviteeRegistered = message.IsInviteeRegistered,
                Message = message.Invite.CustomMessage
            });
        }

    }
}
