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
                VolunteerTasks = CloneVolunteerTasks(@event.VolunteerTasks).ToList(),
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

        private IEnumerable<VolunteerTask> CloneVolunteerTasks(IEnumerable<VolunteerTask> volunteerTasks)
        {
            if (volunteerTasks == null || !volunteerTasks.Any())
            {
                return Enumerable.Empty<VolunteerTask>();
            }
            
            return volunteerTasks.Select(volunteerTask => CloneVolunteerTask(volunteerTask));
        }

        private static IEnumerable<EventSkill> CloneEventRequiredSkills(Event @event)
        {
            return @event.RequiredSkills.Select(eventSkill => new EventSkill { SkillId = eventSkill.SkillId });
        }

        private static VolunteerTask CloneVolunteerTask(VolunteerTask volunteerTask)
        {
            return new VolunteerTask
            {
                Name = volunteerTask.Name,
                Description = volunteerTask.Description,
                Organization = volunteerTask.Organization,
                NumberOfVolunteersRequired = volunteerTask.NumberOfVolunteersRequired,
                StartDateTime = volunteerTask.StartDateTime,
                EndDateTime = volunteerTask.EndDateTime,
                RequiredSkills = CloneVolunteerTaskRequiredSkills(volunteerTask).ToList(),
                IsLimitVolunteers = volunteerTask.IsLimitVolunteers,
                IsAllowWaitList = volunteerTask.IsAllowWaitList
            };
        }

        private static IEnumerable<VolunteerTaskSkill> CloneVolunteerTaskRequiredSkills(VolunteerTask volunteerTask)
        {
            return volunteerTask.RequiredSkills.Select(taskSkill => new VolunteerTaskSkill { SkillId = taskSkill.SkillId });
        }

        private void ApplyUpdates(Event @event, DuplicateEventViewModel updateModel)
        {
            UpdateVolunteerTasks(@event, updateModel);
            UpdateEvent(@event, updateModel);
        }

        static void UpdateVolunteerTasks(Event @event, DuplicateEventViewModel updateModel)
        {
            foreach (var volunteerTask in @event.VolunteerTasks)
            {
                var existingStartDateTime = volunteerTask.StartDateTime;
                var existingEndDateTime = volunteerTask.EndDateTime;

                volunteerTask.StartDateTime = updateModel.StartDateTime - (@event.StartDateTime - volunteerTask.StartDateTime);
                volunteerTask.EndDateTime = volunteerTask.StartDateTime + (existingEndDateTime - existingStartDateTime);
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
                .Include(e => e.VolunteerTasks).ThenInclude(t => t.RequiredSkills)
                .Include(e => e.Organizer)
                .Include(e => e.RequiredSkills)
                .SingleAsync(e => e.Id == eventId);
        }
    }
}