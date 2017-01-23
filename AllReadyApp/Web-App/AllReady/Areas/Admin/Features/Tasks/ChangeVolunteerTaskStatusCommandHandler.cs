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
         var @task = await GetTask(message);
         if (@task == null)
         {
            throw new InvalidOperationException($"Task {message.TaskId} does not exist");
         }
            
         var taskSignup = @task.AssignedVolunteers.SingleOrDefault(c => c.User.Id == message.UserId);
         if (taskSignup == null)
         {
            throw new InvalidOperationException($"Sign-up for user {message.UserId} does not exist");
         }

         switch (message.TaskStatus)
         {
            case VolunteerTaskStatus.Assigned:
               break;
            case VolunteerTaskStatus.Accepted:
               if (taskSignup.Status != VolunteerTaskStatus.Assigned && taskSignup.Status != VolunteerTaskStatus.CanNotComplete && taskSignup.Status != VolunteerTaskStatus.Completed) 
                  throw new ArgumentException("Task must be assigned before being accepted or undoing CanNotComplete or Completed");
               break;
            case VolunteerTaskStatus.Rejected:
               if (taskSignup.Status != VolunteerTaskStatus.Assigned)
                  throw new ArgumentException("Task must be assigned before being rejected");
               break;
            case VolunteerTaskStatus.Completed:
               if (taskSignup.Status != VolunteerTaskStatus.Accepted && taskSignup.Status != VolunteerTaskStatus.Assigned)
                  throw new ArgumentException("Task must be accepted before being completed");
               break;
            case VolunteerTaskStatus.CanNotComplete:
               if (taskSignup.Status != VolunteerTaskStatus.Accepted && taskSignup.Status != VolunteerTaskStatus.Assigned)
                  throw new ArgumentException($"Task must be assigned or accepted before it can be marked as {message.TaskStatus}");
               break;
            default:
               throw new ArgumentException($"Invalid sign-up status value: {message.TaskStatus}");
         }

         taskSignup.Status = message.TaskStatus;
         taskSignup.StatusDateTimeUtc = DateTimeUtcNow();
         taskSignup.StatusDescription = message.TaskStatusDescription;

         await _context.SaveChangesAsync();

         var notification = new TaskSignupStatusChanged { SignupId = taskSignup.Id };
         await _mediator.PublishAsync(notification);
            
         return new VolunteerTaskChangeResult { Status = "success", Task = @task };
      }

      private async Task<VolunteerTask> GetTask(ChangeVolunteerTaskStatusCommand message)
      {
         return await _context.Tasks
            .Include(t => t.AssignedVolunteers).ThenInclude(ts => ts.User)
            .Include(t => t.RequiredSkills).ThenInclude(s => s.Skill)
            .SingleOrDefaultAsync(c => c.Id == message.TaskId);
      }
   }
}