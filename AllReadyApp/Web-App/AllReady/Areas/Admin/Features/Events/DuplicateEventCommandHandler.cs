using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;

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

        Event CloneEvent(Event @event)
        {
            var newEvent = new Event()
            {
                CampaignId = @event.CampaignId,
                Name = @event.Name,
                Description = @event.Description,
                EventType = @event.EventType,
                StartDateTime = @event.StartDateTime,
                EndDateTime = @event.EndDateTime,
                Location = CloneLocation(@event.Location),
                Tasks = CloneTasks(@event.Tasks).ToList(),
                Organizer = @event.Organizer,
                ImageUrl = @event.ImageUrl,
                RequiredSkills = CloneEventRequiredSkills(@event).ToList(),
            };
            return newEvent;
        }

        Location CloneLocation(Location location)
        {
            return new Location()
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

        IEnumerable<AllReadyTask> CloneTasks(IEnumerable<AllReadyTask> tasks)
        {
            if (tasks == null || !tasks.Any())
                return Enumerable.Empty<AllReadyTask>();

            return tasks.Select(task => CloneTask(task));
        }

        static IEnumerable<EventSkill> CloneEventRequiredSkills(Event @event)
        {
            return @event.RequiredSkills.Select(eventSkill => new EventSkill() { SkillId = eventSkill.SkillId });
        }

        AllReadyTask CloneTask(AllReadyTask task)
        {
            return new AllReadyTask()
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

        static IEnumerable<TaskSkill> CloneTaskRequiredSkills(AllReadyTask task)
        {
            return task.RequiredSkills.Select(taskSkill => new TaskSkill { SkillId = taskSkill.SkillId });
        }

        void ApplyUpdates(Event @event, DuplicateEventModel updateModel)
        {
            UpdateTasks(@event, updateModel);
            UpdateEvent(@event, updateModel);
        }

        void UpdateTasks(Event @event, DuplicateEventModel updateModel)
        {
            foreach (var task in @event.Tasks)
            {
                var existingStartDateTime = task.StartDateTime;
                var existingEndDateTime = task.EndDateTime;

                task.StartDateTime = updateModel.StartDateTime - (@event.StartDateTime - task.StartDateTime);
                task.EndDateTime = task.StartDateTime + (existingEndDateTime - existingStartDateTime);
            }
        }

        void UpdateEvent(Event newEvent, DuplicateEventModel model)
        {
            newEvent.Name = model.Name;
            newEvent.Description = model.Description;
            newEvent.StartDateTime = model.StartDateTime;
            newEvent.EndDateTime = model.EndDateTime;
        }

        async Task<Event> GetEvent(int eventId)
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