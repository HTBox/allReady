using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Event;
using AllReady.Areas.Admin.ViewModels.Itinerary;
using AllReady.Areas.Admin.ViewModels.Shared;
using AllReady.Areas.Admin.ViewModels.Task;
using TaskStatus = AllReady.Areas.Admin.Features.Tasks.TaskStatus;

namespace AllReady.Areas.Admin.Features.Events
{
    public class EventDetailQueryHandler : IAsyncRequestHandler<EventDetailQuery, EventDetailViewModel>
    {
        private readonly AllReadyContext _context;

        public EventDetailQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<EventDetailViewModel> Handle(EventDetailQuery message)
        {
            EventDetailViewModel result = null;

            var campaignEvent = await GetEvent(message);

            if (campaignEvent != null)
            {
                result = new EventDetailViewModel
                {
                    Id = campaignEvent.Id,
                    EventType = campaignEvent.EventType,
                    CampaignName = campaignEvent.Campaign.Name,
                    CampaignId = campaignEvent.Campaign.Id,
                    OrganizationId = campaignEvent.Campaign.ManagingOrganizationId,
                    OrganizationName = campaignEvent.Campaign.ManagingOrganization.Name,
                    Name = campaignEvent.Name,
                    Description = campaignEvent.Description,
                    TimeZoneId = campaignEvent.TimeZoneId,
                    StartDateTime = campaignEvent.StartDateTime,
                    EndDateTime = campaignEvent.EndDateTime,
                    IsLimitVolunteers = campaignEvent.IsLimitVolunteers,
                    IsAllowWaitList = campaignEvent.IsAllowWaitList,
                    Location = campaignEvent.Location.ToEditModel(),                    
                    ImageUrl = campaignEvent.ImageUrl,
                    Tasks = campaignEvent.Tasks.Select(t => new TaskSummaryViewModel
                    {
                        Id = t.Id,
                        Name = t.Name,
                        StartDateTime = t.StartDateTime,
                        EndDateTime = t.EndDateTime,
                        NumberOfVolunteersRequired = t.NumberOfVolunteersRequired,
                        AssignedVolunteers = t.AssignedVolunteers?.Select(assignedVolunteer => new VolunteerViewModel
                        {
                            UserId = assignedVolunteer.User.Id,
                            UserName = assignedVolunteer.User.UserName,
                            HasVolunteered = true,
                            Status = assignedVolunteer.Status,
                            AdditionalInfo = assignedVolunteer.AdditionalInfo
                        }).ToList()
                    }).OrderBy(t => t.StartDateTime).ThenBy(t => t.Name).ToList(),
                    Itineraries = campaignEvent.Itineraries.Select(i => new ItineraryListViewModel
                    {
                        Id = i.Id,
                        Name = i.Name,
                        Date = i.Date,
                        TeamSize = i.TeamMembers.Count,
                        RequestCount = i.Requests.Count
                    }).OrderBy(i => i.Date).ToList()
                };


                // required skills

                var skillIds = campaignEvent.RequiredSkills.Select(s => s.SkillId);

                var skillNames = await _context.Skills.AsNoTracking()
                    .Include(s => s.ParentSkill)
                    .Include(s => s.ChildSkills)
                    .Where(s => skillIds.Contains(s.Id))
                    .Where(s => s.HierarchicalName != Skill.InvalidHierarchy)
                    .ToListAsync();

                result.RequiredSkillNames = skillNames.Select(s => s.HierarchicalName).ToList();

                // end required skills

                result.NewItinerary.EventId = result.Id;
                result.NewItinerary.Date = result.StartDateTime.DateTime;

                var requests = await _context.Requests
                    .AsNoTracking()
                    .Where(rec => rec.EventId == message.EventId)
                    .GroupBy(rec => rec.Status)
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToListAsync();

                foreach (var req in requests)
                {
                    switch (req.Status)
                    {
                        case RequestStatus.Completed:
                            result.CompletedRequests = req.Count;
                            break;
                        case RequestStatus.Assigned:
                            result.AssignedRequests = req.Count;
                            break;
                        case RequestStatus.Canceled:
                            result.CanceledRequests = req.Count;
                            break;
                        case RequestStatus.Unassigned:
                            result.UnassignedRequests = req.Count;
                            break;
                    }
                }

                result.TotalRequests = result.CompletedRequests + result.CanceledRequests + result.AssignedRequests +
                                       result.UnassignedRequests;

                result.VolunteersRequired = await _context.Tasks.Where(rec => rec.EventId == result.Id).SumAsync(rec => rec.NumberOfVolunteersRequired);

                var acceptedVolunteers = await _context.Tasks
                    .AsNoTracking()
                    .Include(rec => rec.AssignedVolunteers)
                    .Where(rec => rec.EventId == result.Id)
                    .ToListAsync();

                result.AcceptedVolunteers = acceptedVolunteers.Sum(x => x.AssignedVolunteers.Where(v => v.Status == TaskStatus.Accepted.ToString()).Count());
            }

            return result;
        }

        private async Task<Event> GetEvent(EventDetailQuery message)
        {
            return await _context.Events
                .AsNoTracking()
                .Include(a => a.Campaign).ThenInclude(c => c.ManagingOrganization)
                .Include(a => a.Tasks).ThenInclude(t => t.AssignedVolunteers).ThenInclude(av => av.User)
                .Include(a => a.RequiredSkills).ThenInclude(s => s.Skill).ThenInclude(s => s.ParentSkill)
                .Include(a => a.Location)
                .Include(a => a.Itineraries).ThenInclude(a => a.TeamMembers)
                .Include(a => a.Itineraries).ThenInclude(a => a.Requests)
                .SingleOrDefaultAsync(a => a.Id == message.EventId);
        }
    }
}