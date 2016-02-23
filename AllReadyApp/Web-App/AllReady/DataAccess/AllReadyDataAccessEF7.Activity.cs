using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;

namespace AllReady.Models
{
    public partial class AllReadyDataAccessEF7 : IAllReadyDataAccess
    {
        IEnumerable<Activity> IAllReadyDataAccess.Activities
        {
            get
            {
                return _dbContext.Activities
                                .Include(a => a.Location)
                                .Include(a => a.Location.PostalCode)
                                .Include(a => a.Campaign)
                                .Include(a => a.Campaign.ManagingOrganization)
                                .Include(a => a.Tasks)
                                .Include(a => a.RequiredSkills)
                                .Include(a => a.UsersSignedUp)
                                .OrderBy(a => a.EndDateTime)
                                .ToList();
            }
        }
       
        Task IAllReadyDataAccess.UpdateActivity(Activity value)
        {
            //First remove any skills that are no longer associated with this activity
            var acsksToRemove = _dbContext.ActivitySkills.Where(acsk => acsk.ActivityId == value.Id && (value.RequiredSkills == null ||
                !value.RequiredSkills.Any(acsk1 => acsk1.SkillId == acsk.SkillId)));
            _dbContext.ActivitySkills.RemoveRange(acsksToRemove);
            _dbContext.Activities.Update(value);
            return _dbContext.SaveChangesAsync();
        }

        IEnumerable<Activity> IAllReadyDataAccess.ActivitiesByPostalCode(string postalCode, int distance)
        {
            return _dbContext.Activities.FromSql("EXEC GetClosestActivitiesByPostalCode '{0}', {1}, {2}", postalCode, 50, distance)
                .Include(a => a.Campaign);
        }

        IEnumerable<Activity> IAllReadyDataAccess.ActivitiesByGeography(double latitude, double longitude, int distance)
        {
            return _dbContext.Activities.FromSql("EXEC GetClosestActivities {0}, {1}, {2}, {3}", latitude, longitude, 50, distance);
        }

        Activity IAllReadyDataAccess.GetActivity(int activityId)
        {
            return _dbContext.Activities
                .Include(a => a.Location)
                .Include(a => a.Location.PostalCode)
                .Include(a => a.Campaign)
                .Include(a => a.RequiredSkills).ThenInclude(rs => rs.Skill).ThenInclude(s => s.ParentSkill)
                .Include(a => a.Tasks).ThenInclude(t => t.AssignedVolunteers).ThenInclude(tu => tu.User)
                .Include(a => a.UsersSignedUp).ThenInclude(u => u.User)
                .SingleOrDefault(a => a.Id == activityId);
        }

        int IAllReadyDataAccess.GetManagingOrganizationId(int activityId)
        {
            return _dbContext.Activities.Where(a => a.Id == activityId).Select(a => a.Campaign.ManagingOrganizationId).FirstOrDefault();
        }

        IEnumerable<ActivitySignup> IAllReadyDataAccess.GetActivitySignups(int activityId, string userId)
        {
            return _dbContext.ActivitySignup
                        .Include(x => x.User)
                        .Include(x => x.Activity)
                        .Include(x => x.Activity.Campaign)                        
                        .Where(x => x.Activity.Id == activityId && x.User.Id == userId)
                        .OrderBy(x => x.Activity.StartDateTime)
                        .ToArray();
        }

        IEnumerable<ActivitySignup> IAllReadyDataAccess.GetActivitySignups(string userId)
        {
            return _dbContext.ActivitySignup
                        .Include(x => x.User)
                        .Include(x => x.Activity)
                        .Include(x => x.Activity.Campaign)                        
                        .Where(x => x.User.Id == userId)
                        .OrderBy(x => x.Activity.StartDateTime)
                        .ToArray();
        }

        IEnumerable<TaskSignup> IAllReadyDataAccess.GetTasksAssignedToUser(int activityId, string userId)
        {
            var unfilteredTasks = _dbContext.TaskSignups
                .Include(ts => ts.Task)
                .ThenInclude(t => t.Activity)
                .ThenInclude(t => t.Campaign)
                .Include(ts => ts.User)                
                .ToList();

            var finalTasks = unfilteredTasks.Where(ts => ts.Task.Activity.Id == activityId && ts.User.Id == userId && !ts.Task.Activity.Campaign.Locked).ToList();

            return finalTasks;
        }
        IEnumerable<Resource> IAllReadyDataAccess.GetResourcesByCategory(string category)
        {
            var resources = from c in _dbContext.Resources
                            select c;
            return resources;
        }

        Task IAllReadyDataAccess.UpdateCampaign(Campaign value)
        {
            _dbContext.Campaigns.Update(value);
            return _dbContext.SaveChangesAsync();
        }
    }
}
