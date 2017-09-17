using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using AllReady.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class RemoveVolunteerFromTaskCommandHandler : AsyncRequestHandler<RemoveVolunteerFromTaskCommand>
    {
        private AllReadyContext _context;
        private readonly IEmailSender _emailSender;

        public RemoveVolunteerFromTaskCommandHandler(AllReadyContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }


        protected override async Task HandleCore(RemoveVolunteerFromTaskCommand message)
        {
            var row = await _context.VolunteerTaskSignups.FirstOrDefaultAsync(x =>
                x.VolunteerTaskId == message.TaskId
                && x.User.Id == message.UserId);

            if (row == null)
                throw new InvalidOperationException("User " + message.UserId + " is not a volunteer for task " + message.TaskId);

            _context.VolunteerTaskSignups.Remove(row);
            await _context.SaveChangesAsync();

            if (!message.NotifyUser)
                return;

            var user = await _context.Users.FirstAsync(x => x.Id == message.UserId);
            var task = await _context.VolunteerTasks.FirstAsync(x => x.Id == message.TaskId);
            await _emailSender.SendEmailAsync(user.Email,
                "Removed from a volunteer task",
                $"The administrator has removed you from task '{task.Name}'.");

        }
    }
}
