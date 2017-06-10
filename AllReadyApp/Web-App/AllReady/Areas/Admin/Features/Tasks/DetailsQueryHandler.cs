using System.Linq;
using System.Threading.Tasks;

using AllReady.Areas.Admin.ViewModels.Shared;
using AllReady.Areas.Admin.ViewModels.VolunteerTask;
using AllReady.Models;

using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class DetailsQueryHandler : IAsyncRequestHandler<DetailsQuery, DetailsViewModel>
    {
        private readonly AllReadyContext _context;

        public DetailsQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<DetailsViewModel> Handle(DetailsQuery message)
        {
            var volunteerTask = await GetVolunteerTask(message);

            var model = new DetailsViewModel
            {
                Id = volunteerTask.Id,
                Name = volunteerTask.Name,
                Description = volunteerTask.Description,
                StartDateTime = volunteerTask.StartDateTime,
                EndDateTime = volunteerTask.EndDateTime,
                NumberOfVolunteersRequired = volunteerTask.NumberOfVolunteersRequired,
                EventId = volunteerTask.Event.Id,
                EventName = volunteerTask.Event.Name,
                CampaignId = volunteerTask.Event.CampaignId,
                CampaignName = volunteerTask.Event.Campaign.Name,
                TimeZoneId = volunteerTask.Event.TimeZoneId,
                RequiredSkills = volunteerTask.RequiredSkills,
                AssignedVolunteers = volunteerTask.AssignedVolunteers.Select(ts => new VolunteerViewModel
                {
                    UserId = ts.User.Id,
                    UserName = ts.User.UserName,
                    HasVolunteered = true,
                    Name = ts.User.Name,
                    PhoneNumber = ts.User.PhoneNumber,
                    AssociatedSkills = ts.User.AssociatedSkills,
                }).ToList(),
                Attachments = volunteerTask.Attachments.Select(a => new FileAttachment
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    Url = a.Url,
                }).ToList(),
            };

            return model;
        }

        private async Task<VolunteerTask> GetVolunteerTask(DetailsQuery message)
        {
            return await _context.VolunteerTasks
                .AsNoTracking()
                .Include(t => t.Event)
                .Include(t => t.Event.Campaign)
                .Include(t => t.Attachments)
                .Include(t => t.AssignedVolunteers).ThenInclude(ts => ts.User).ThenInclude(u => u.AssociatedSkills).ThenInclude(s => s.Skill).ThenInclude(s => s.ParentSkill).ThenInclude(s => s.ParentSkill)
                .Include(t => t.RequiredSkills).ThenInclude(ts => ts.Skill).ThenInclude(s => s.ParentSkill).ThenInclude(s => s.ParentSkill)
                .SingleOrDefaultAsync(t => t.Id == message.VolunteerTaskId);
        }
    }
}