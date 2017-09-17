using AllReady.Areas.Admin.ViewModels.Event;
using AllReady.Areas.Admin.ViewModels.Itinerary;
using AllReady.Areas.Admin.ViewModels.Shared;
using AllReady.Areas.Admin.ViewModels.VolunteerTask;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

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
                    VolunteerTasks = campaignEvent.VolunteerTasks.Select(t => new TaskSummaryViewModel
                    {
                        Id = t.Id,
                        Name = t.Name,
                        StartDateTime = t.StartDateTime,
                        EndDateTime = t.EndDateTime,
                        EventId = campaignEvent.Id,
                        NumberOfVolunteersRequired = t.NumberOfVolunteersRequired,
                        AssignedVolunteers = t.AssignedVolunteers?.Select(assignedVolunteer => new VolunteerViewModel
                        {
                            UserId = assignedVolunteer.User.Id,
                            UserName = assignedVolunteer.User.UserName,
                            HasVolunteered = true,
                            Status = assignedVolunteer.Status.ToString(),
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
                    }).OrderBy(i => i.Date).ToList(),
                    EventManagerInvites = campaignEvent.ManagementInvites.Select(i => new EventDetailViewModel.EventManagerInviteList
                    {
                        Id = i.Id,
                        InviteeEmail = i.InviteeEmailAddress,
                        Status = GetEventManagerInviteStatus(i)
                    })
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

                foreach (var request in requests)
                {
                    switch (request.Status)
                    {
                        case RequestStatus.Unassigned:
                            result.UnassignedRequests = request.Count;
                            break;
                        case RequestStatus.Assigned:
                            result.AssignedRequests = request.Count;
                            break;
                        case RequestStatus.PendingConfirmation:
                            result.PendingConfirmationRequests = request.Count;
                            break;
                        case RequestStatus.Confirmed:
                            result.ConfirmedRequests = request.Count;
                            break;
                        case RequestStatus.Completed:
                            result.CompletedRequests = request.Count;
                            break;
                        case RequestStatus.Canceled:
                            result.CanceledRequests = request.Count;
                            break;
                        case RequestStatus.Requested:
                            result.RequestedRequests = request.Count;
                            break;
                    }
                }

                result.TotalRequests = 
                    result.UnassignedRequests +
                    result.AssignedRequests +
                    result.PendingConfirmationRequests +
                    result.ConfirmedRequests +
                    result.CompletedRequests + 
                    result.CanceledRequests +
                    result.RequestedRequests;

                result.VolunteersRequired = await _context.VolunteerTasks.Where(rec => rec.EventId == result.Id).SumAsync(rec => rec.NumberOfVolunteersRequired);

                var acceptedVolunteers = await _context.VolunteerTasks
                    .AsNoTracking()
                    .Include(rec => rec.AssignedVolunteers)
                    .Where(rec => rec.EventId == result.Id)
                    .ToListAsync();

                result.AcceptedVolunteers = acceptedVolunteers.Sum(x => x.AssignedVolunteers.Count(v => v.Status == VolunteerTaskStatus.Accepted));
            }

            return result;
        }

        private async Task<Event> GetEvent(EventDetailQuery message)
        {
            return await _context.Events
                .AsNoTracking()
                .Include(a => a.Campaign).ThenInclude(c => c.ManagingOrganization)
                .Include(a => a.VolunteerTasks).ThenInclude(t => t.AssignedVolunteers).ThenInclude(av => av.User)
                .Include(a => a.RequiredSkills).ThenInclude(s => s.Skill).ThenInclude(s => s.ParentSkill)
                .Include(a => a.Location)
                .Include(a => a.Itineraries).ThenInclude(a => a.TeamMembers)
                .Include(a => a.Itineraries).ThenInclude(a => a.Requests)
                .Include(a => a.ManagementInvites)
                .SingleOrDefaultAsync(a => a.Id == message.EventId);
        }

        private EventDetailViewModel.EventManagerInviteStatus GetEventManagerInviteStatus(AllReady.Models.EventManagerInvite eventManagerInvite)
        {
            if (eventManagerInvite.IsAccepted) return EventDetailViewModel.EventManagerInviteStatus.Accepted;
            else if (eventManagerInvite.IsPending) return EventDetailViewModel.EventManagerInviteStatus.Pending;
            else if (eventManagerInvite.IsRejected) return EventDetailViewModel.EventManagerInviteStatus.Rejected;
            else return EventDetailViewModel.EventManagerInviteStatus.Revoked;
        }
    }
}
