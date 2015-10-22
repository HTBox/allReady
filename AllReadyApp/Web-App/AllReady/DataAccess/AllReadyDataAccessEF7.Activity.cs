using System;
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
                                .Include(a => a.Tenant)
                                .Include(a => a.Campaign)
                                .Include(a => a.Tasks)
                                .Include(x => x.UsersSignedUp)
                                .OrderBy(x => x.EndDateTimeUtc)
                                .ToList();
            }
        }
        Task IAllReadyDataAccess.AddActivity(Activity value)
        {
            _dbContext.Activities.Add(value);
            return _dbContext.SaveChangesAsync();
        }

        Task IAllReadyDataAccess.DeleteActivity(int id)
        {
            var activity = _dbContext.Activities.SingleOrDefault(c => c.Id == id);

            if (activity != null)
            {
                _dbContext.Activities.Remove(activity);
                return _dbContext.SaveChangesAsync();
            }
            return null;
        }

        Task IAllReadyDataAccess.UpdateActivity(Activity value)
        {
            //First remove any skills that are no longer associated with this activity
            var acsksToRemove = _dbContext.ActivitySkills.Where(acsk => acsk.ActivityId == value.Id && !value.RequiredSkills.Any(acsk1 => acsk1.SkillId == acsk.SkillId));
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
                .Include(a => a.Tenant)
                .Include(a => a.Campaign)
                .Include(a => a.RequiredSkills).ThenInclude(acsk => acsk.Skill).ThenInclude(sk => sk.ParentSkill)
                .Include(a => a.Tasks).ThenInclude(t => t.AssignedVolunteers).ThenInclude(tu => tu.User)
                .Include(a => a.UsersSignedUp).ThenInclude(u => u.User)
                .SingleOrDefault(a => a.Id == activityId);
        }

        IEnumerable<ActivitySignup> IAllReadyDataAccess.GetActivitySignups(int activityId, ApplicationUser user)
        {
            return _dbContext.ActivitySignup
                        .Include(x => x.User)
                        .Include(x => x.Activity)
                        .ToArray()
                        .Where(x => x.Activity.Id == activityId && x.User.Id == user.Id)
                        .OrderBy(x => x.Activity.StartDateTimeUtc);
        }

        IEnumerable<ActivitySignup> IAllReadyDataAccess.GetActivitySignups(ApplicationUser user)
        {
            return _dbContext.ActivitySignup
                        .Include(x => x.User)
                        .Include(x => x.Activity)
                        .ToArray()
                        .Where(x => x.User.Id == user.Id)
                        .OrderBy(x => x.Activity.StartDateTimeUtc);
        }

        IEnumerable<TaskUsers> IAllReadyDataAccess.GetTasksAssignedToUser(int activityId, ApplicationUser user)
        {
            var unfilteredTasks = _dbContext.TaskSignup
                .Include(ts => ts.Task)
                .ThenInclude(t => t.Activity)
                .Include(ts => ts.User)
                .ToList();

            var finalTasks = unfilteredTasks.Where(ts => ts.Task.Activity.Id == activityId && ts.User.Id == user.Id).ToList();

            return finalTasks;
        }
        IEnumerable<Resource> IAllReadyDataAccess.GetResourcesByCategory(string category)
        {
            var resources = from c in _dbContext.Resources
                            select c;
            return resources;
        }
    }
}
