using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;
using System.Linq;

namespace AllReady.Areas.Admin.Features.Activities
{
    public class EditActivityCommandHandler : IRequestHandler<EditActivityCommand, int>
    {
        private AllReadyContext _context;

        public EditActivityCommandHandler(AllReadyContext context)
        {
            _context = context;

        }
        public int Handle(EditActivityCommand message)
        {
            var activity = 
                _context.Activities
                .Include(a => a.RequiredSkills)
                .SingleOrDefault(c => c.Id == message.Activity.Id);

            if (activity == null)
            {
                activity = new Activity();
            }

            activity.Name = message.Activity.Name;
            activity.Description = message.Activity.Description;
            activity.StartDateTimeUtc = message.Activity.StartDateTime;
            activity.EndDateTimeUtc = message.Activity.EndDateTime;
            activity.CampaignId = message.Activity.CampaignId;
            activity.NumberOfVolunteersRequired = message.Activity.NumberOfVolunteersRequired;

            if (activity.Id > 0)
            {
                var skillsToRemove = _context.ActivitySkills.Where(skill => skill.ActivityId == activity.Id && (message.Activity.RequiredSkills == null ||
                    !message.Activity.RequiredSkills.Any(ts1 => ts1.SkillId == skill.SkillId)));
                _context.ActivitySkills.RemoveRange(skillsToRemove);
            }
            if (message.Activity.RequiredSkills != null)
            {
                activity.RequiredSkills.AddRange(message.Activity.RequiredSkills.Where(mt => !activity.RequiredSkills.Any(ts => ts.SkillId == mt.SkillId)));
            }
            
            _context.Update(activity);
            _context.SaveChanges();
            return activity.Id;
        }
    }
}
