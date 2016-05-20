using System;
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
        private AllReadyContext _context;

        public DuplicateEventCommandHandler(AllReadyContext context)
        {
            _context = context;

        }
        public async Task<int> Handle(DuplicateEventCommand message)
        {
            var eventToDuplicate = await GetEvent(message.DuplicateEventModel.Id);
            var newEvent = duplicateEvent(eventToDuplicate, message.DuplicateEventModel);
            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return newEvent.Id;
        }

        private Event duplicateEvent(Event eventToDuplicate, DuplicateEventModel model)
        {
            Event newEvent;
            newEvent = cloneEvent(eventToDuplicate);
            updateEvent(newEvent, model);
            cloneEventTasks(newEvent, eventToDuplicate);
            cloneEventLocation(newEvent, eventToDuplicate);

            return newEvent;
        }

        private Event cloneEvent(Event eventToDuplicate)
        {
            return new Event()
            {
                CampaignId = eventToDuplicate.CampaignId,
                Name = eventToDuplicate.Name,
                Description = eventToDuplicate.Description,
                EventType = eventToDuplicate.EventType,
                NumberOfVolunteersRequired = eventToDuplicate.NumberOfVolunteersRequired,
                StartDateTime = eventToDuplicate.StartDateTime,
                EndDateTime = eventToDuplicate.EndDateTime,
                Organizer = eventToDuplicate.Organizer,
                ImageUrl = eventToDuplicate.ImageUrl,
                IsLimitVolunteers = eventToDuplicate.IsLimitVolunteers,
                IsAllowWaitList = eventToDuplicate.IsAllowWaitList
            };
        }

        private void updateEvent(Event newEvent, DuplicateEventModel model)
        {
            newEvent.Name = model.Name;
            newEvent.Description = model.Description;
            newEvent.StartDateTime = model.StartDateTime;
            newEvent.EndDateTime = model.EndDateTime;
        }

        private void cloneEventTasks(Event newEvent, Event eventToDuplicate)
        {
            if (eventToDuplicate.Tasks == null || !eventToDuplicate.Tasks.Any())
                return;

            foreach (var task in eventToDuplicate.Tasks)
            {
                var newTask = cloneTask(task);
                newTask.Event = newEvent;

                // Todo: Check if this handles timezones correctly.
                if (task.StartDateTime.HasValue)
                    newTask.StartDateTime = newEvent.StartDateTime - (eventToDuplicate.StartDateTime - task.StartDateTime.Value);

                if (task.EndDateTime.HasValue && task.StartDateTime.HasValue && newTask.StartDateTime.HasValue)
                    newTask.EndDateTime = newTask.StartDateTime + (task.EndDateTime - task.StartDateTime);

                _context.Tasks.Add(newTask);
            }
        }

        private AllReadyTask cloneTask(AllReadyTask task)
        {
            return new AllReadyTask()
            {
                Name = task.Name,
                Description = task.Description,
                Organization = task.Organization,
                NumberOfVolunteersRequired = task.NumberOfVolunteersRequired,
                StartDateTime = task.StartDateTime,
                EndDateTime = task.EndDateTime,
                RequiredSkills = task.RequiredSkills,
                IsLimitVolunteers = task.IsLimitVolunteers,
                IsAllowWaitList = task.IsAllowWaitList
            };
        }

        private void cloneEventLocation(Event newEvent, Event eventToDuplicate)
        {
            var newLocation = new Location()
            {
                Address1 = eventToDuplicate.Location.Address1,
                Address2 = eventToDuplicate.Location.Address2,
                City = eventToDuplicate.Location.City,
                State = eventToDuplicate.Location.State,
                PostalCode = eventToDuplicate.Location.PostalCode,
                Name = eventToDuplicate.Location.Name,
                PhoneNumber = eventToDuplicate.Location.PhoneNumber,
                Country = eventToDuplicate.Location.Country
            };

            newEvent.Location = newLocation;
            _context.Locations.Add(newLocation);
        }

        private async Task<Event> GetEvent(int eventId)
        {
            return await _context.Events
                .AsNoTracking()
                .Include(e => e.Location)
                .Include(e => e.Tasks)
                .SingleOrDefaultAsync(e => e.Id == eventId);
            //.ConfigureAwait(false); // Todo: Check what this is and if its needed.
        }
    }
}