using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Event;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Events
{
    public class DuplicateEventCommandHandler : IAsyncRequestHandler<DuplicateEventCommand, int>
    {
        AllReadyContext _context;

        public DuplicateEventCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(DuplicateEventCommand message)
        {
            var @event = await GetEvent(message.DuplicateEventModel.Id);
            var newEvent = CloneEvent(@event);
            ApplyUpdates(newEvent, message.DuplicateEventModel);
            _context.Add(newEvent.Location);
            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();
            return newEvent.Id;
        }

        private Event CloneEvent(Event @event)
        {
            var newEvent = new Event
            {
                CampaignId = @event.CampaignId,
                Name = @event.Name,
                Description = @event.Description,
                EventType = @event.EventType,
                StartDateTime = @event.StartDateTime,
                TimeZoneId = @event.TimeZoneId,
                EndDateTime = @event.EndDateTime,
                Location = CloneLocation(@event.Location),
                Tasks = CloneTasks(@event.Tasks).ToList(),
                Organizer = @event.Organizer,
                ImageUrl = @event.ImageUrl,
                RequiredSkills = CloneEventRequiredSkills(@event).ToList(),
                IsLimitVolunteers = @event.IsLimitVolunteers,
                IsAllowWaitList = @event.IsAllowWaitList
            };
            return newEvent;
        }

        private static Location CloneLocation(Location location)
        {
            return new Location
            {
                Address1 = location.Address1,
                Address2 = location.Address2,
                City = location.City,
                State = location.State,
                PostalCode = location.PostalCode,
                Name = location.Name,
                PhoneNumber = location.PhoneNumber,
                Country = location.Country
            };
        }

        private IEnumerable<AllReadyTask> CloneTasks(IEnumerable<AllReadyTask> tasks)
        {
            if (tasks == null || !tasks.Any())
            {
                return Enumerable.Empty<AllReadyTask>();
            }
            
            return tasks.Select(task => CloneTask(task));
        }

        private static IEnumerable<EventSkill> CloneEventRequiredSkills(Event @event)
        {
            return @event.RequiredSkills.Select(eventSkill => new EventSkill { SkillId = eventSkill.SkillId });
        }

        private static AllReadyTask CloneTask(AllReadyTask task)
        {
            return new AllReadyTask
            {
                Name = task.Name,
                Description = task.Description,
                Organization = task.Organization,
                NumberOfVolunteersRequired = task.NumberOfVolunteersRequired,
                StartDateTime = task.StartDateTime,
                EndDateTime = task.EndDateTime,
                RequiredSkills = CloneTaskRequiredSkills(task).ToList(),
                IsLimitVolunteers = task.IsLimitVolunteers,
                IsAllowWaitList = task.IsAllowWaitList
            };
        }

        private static IEnumerable<TaskSkill> CloneTaskRequiredSkills(AllReadyTask task)
        {
            return task.RequiredSkills.Select(taskSkill => new TaskSkill { SkillId = taskSkill.SkillId });
        }

        private void ApplyUpdates(Event @event, DuplicateEventViewModel updateModel)
        {
            UpdateTasks(@event, updateModel);
            UpdateEvent(@event, updateModel);
        }

        static void UpdateTasks(Event @event, DuplicateEventViewModel updateModel)
        {
            foreach (var task in @event.Tasks)
            {
                var existingStartDateTime = task.StartDateTime;
                var existingEndDateTime = task.EndDateTime;

                task.StartDateTime = updateModel.StartDateTime - (@event.StartDateTime - task.StartDateTime);
                task.EndDateTime = task.StartDateTime + (existingEndDateTime - existingStartDateTime);
            }
        }

        static void UpdateEvent(Event newEvent, DuplicateEventViewModel model)
        {
            newEvent.Name = model.Name;
            newEvent.Description = model.Description;
            newEvent.StartDateTime = model.StartDateTime;
            newEvent.EndDateTime = model.EndDateTime;
        }

        private async Task<Event> GetEvent(int eventId)
        {
            return await _context.Events
                .AsNoTracking()
                .Include(e => e.Location)
                .Include(e => e.Tasks).ThenInclude(t => t.RequiredSkills)
                .Include(e => e.Organizer)
                .Include(e => e.RequiredSkills)
                .SingleAsync(e => e.Id == eventId);
        }
    }
}