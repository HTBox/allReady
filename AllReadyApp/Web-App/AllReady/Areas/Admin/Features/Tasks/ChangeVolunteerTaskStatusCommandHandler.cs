using System;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Tasks
{
   public class ChangeVolunteerTaskStatusCommandHandler : IAsyncRequestHandler<ChangeVolunteerTaskStatusCommand, VolunteerTaskChangeResult>
   {
      public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

      private readonly AllReadyContext _context;
      private readonly IMediator _mediator;

      public ChangeVolunteerTaskStatusCommandHandler(AllReadyContext context, IMediator mediator)
      {
         _context = context;
         _mediator = mediator;
      }

      public async Task<VolunteerTaskChangeResult> Handle(ChangeVolunteerTaskStatusCommand message)
      {
         var volunteerTask = await GetVolunteerTask(message);
         if (volunteerTask == null)
         {
            throw new InvalidOperationException($"Task {message.VolunteerTaskId} does not exist");
         }
            
         var volunteerTaskSignup = volunteerTask.AssignedVolunteers.SingleOrDefault(c => c.User.Id == message.UserId);
         if (volunteerTaskSignup == null)
         {
            throw new InvalidOperationException($"Sign-up for user {message.UserId} does not exist");
         }

         switch (message.VolunteerTaskStatus)
         {
            case VolunteerTaskStatus.Assigned:
               break;
            case VolunteerTaskStatus.Accepted:
               if (volunteerTaskSignup.Status != VolunteerTaskStatus.Assigned && volunteerTaskSignup.Status != VolunteerTaskStatus.CanNotComplete && volunteerTaskSignup.Status != VolunteerTaskStatus.Completed) 
                  throw new ArgumentException("Task must be assigned before being accepted or undoing CanNotComplete or Completed");
               break;
            case VolunteerTaskStatus.Rejected:
               if (volunteerTaskSignup.Status != VolunteerTaskStatus.Assigned)
                  throw new ArgumentException("Task must be assigned before being rejected");
               break;
            case VolunteerTaskStatus.Completed:
               if (volunteerTaskSignup.Status != VolunteerTaskStatus.Accepted && volunteerTaskSignup.Status != VolunteerTaskStatus.Assigned)
                  throw new ArgumentException("Task must be accepted before being completed");
               break;
            case VolunteerTaskStatus.CanNotComplete:
               if (volunteerTaskSignup.Status != VolunteerTaskStatus.Accepted && volunteerTaskSignup.Status != VolunteerTaskStatus.Assigned)
                  throw new ArgumentException($"Task must be assigned or accepted before it can be marked as {message.VolunteerTaskStatus}");
               break;
            default:
               throw new ArgumentException($"Invalid sign-up status value: {message.VolunteerTaskStatus}");
         }

         volunteerTaskSignup.Status = message.VolunteerTaskStatus;
         volunteerTaskSignup.StatusDateTimeUtc = DateTimeUtcNow();
         volunteerTaskSignup.StatusDescription = message.VolunteerTaskStatusDescription;

         await _context.SaveChangesAsync();

         var notification = new VolunteerTaskSignupStatusChanged { SignupId = volunteerTaskSignup.Id };
         await _mediator.PublishAsync(notification);
            
         return new VolunteerTaskChangeResult { Status = "success", VolunteerTask = volunteerTask };
      }

      private async Task<VolunteerTask> GetVolunteerTask(ChangeVolunteerTaskStatusCommand message)
      {
         return await _context.VolunteerTasks
            .Include(t => t.AssignedVolunteers).ThenInclude(ts => ts.User)
            .Include(t => t.RequiredSkills).ThenInclude(s => s.Skill)
            .SingleOrDefaultAsync(c => c.Id == message.VolunteerTaskId);
      }
   }
}