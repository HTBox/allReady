using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Relational;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrepOps.Models
{
    public class PrepOpsDataAccessEF7 : IPrepOpsDataAccess
    {
        #region Fields, Constructors, Context setup

        private static bool _databaseChecked = false;
        private readonly PrepOpsContext _dbContext;
        public PrepOpsDataAccessEF7(PrepOpsContext dbContext)
        {
            _dbContext = dbContext;
        }

        // The following code creates the database and schema if they don't exist.
        // This is a temporary workaround since deploying database through EF migrations is not yet supported in this release.
        // Please see this http://go.microsoft.com/fwlink/?LinkID=615859 for more information on how to do deploy the database
        // when publishing your application.
        public void EnsureDatabaseCreated()
        {
            if (!_databaseChecked)
            {
                _databaseChecked = true;

                if (_dbContext.Database is RelationalDatabase)
                {
                    _dbContext.Database.AsRelational().ApplyMigrations();
                }
            }
        }

        #endregion

        #region Activity CRUD
        IEnumerable<Activity> IPrepOpsDataAccess.Activities
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
                                .ToList();
            }
        }
        Task IPrepOpsDataAccess.AddActivity(Activity value)
        {
            _dbContext.Activities.Add(value);
            return _dbContext.SaveChangesAsync();
        }

        Task IPrepOpsDataAccess.DeleteActivity(int id)
        {
            var activity = _dbContext.Activities.SingleOrDefault(c => c.Id == id);

            if (activity != null)
            {
                _dbContext.Activities.Remove(activity);
                return _dbContext.SaveChangesAsync();
            }
            return null;
        }

        Task IPrepOpsDataAccess.UpdateActivity(Activity value)
        {
            _dbContext.Activities.Update(value);
            return _dbContext.SaveChangesAsync();
        }

        IEnumerable<Activity> IPrepOpsDataAccess.ActivitiesByPostalCode(string postalCode, int distance)
        {
            return _dbContext.Activities.FromSql("EXEC GetClosestActivitiesByPostalCode '{0}', {1}, {2}", postalCode, 50, distance);
        }

        IEnumerable<Activity> IPrepOpsDataAccess.ActivitiesByGeography(double latitude, double longitude, int distance)
        {
            return _dbContext.Activities.FromSql("EXEC GetClosestActivities {0}, {1}, {2}, {3}", latitude, longitude, 50, distance);
        }

        Activity IPrepOpsDataAccess.GetActivity(int activityId)
        {
            return _dbContext.Activities
                .Include(a => a.Location)
                .Include(a => a.Location.PostalCode)
                .Include(a => a.Tenant)
                .Include(a => a.Campaign)
                .Include(a => a.Tasks).ThenInclude(t => t.AssignedVolunteers).ThenInclude(tu => tu.User)
                .Include(a => a.UsersSignedUp).ThenInclude(u => u.User)
                .SingleOrDefault(a => a.Id == activityId);
        }

        IEnumerable<ActivitySignup> IPrepOpsDataAccess.GetActivitySignups(int activityId, ApplicationUser user)
        {
            return _dbContext.ActivitySignup
                        .Include(x => x.User)
                        .Include(x => x.Activity)
                        .ToArray()
                        .Where(x => x.Activity.Id == activityId && x.User.Id == user.Id)
                        .OrderBy(x => x.Activity.StartDateTimeUtc);
        }

        IEnumerable<ActivitySignup> IPrepOpsDataAccess.GetActivitySignups(ApplicationUser user)
        {
            return _dbContext.ActivitySignup
                        .Include(x => x.User)
                        .Include(x => x.Activity)
                        .ToArray()
                        .Where(x => x.User.Id == user.Id)
                        .OrderBy(x => x.Activity.StartDateTimeUtc);
        }

        IEnumerable<TaskUsers> IPrepOpsDataAccess.GetTasksAssignedToUser(int activityId, ApplicationUser user) {
            var unfilteredTasks = _dbContext.TaskSignup
                .Include(ts => ts.Task)
                .ThenInclude(t => t.Activity)
                .Include(ts => ts.User)
                .ToList();

            var finalTasks = unfilteredTasks.Where(ts => ts.Task.Activity.Id == activityId && ts.User.Id == user.Id).ToList();

            return finalTasks;
        }


        #endregion

        #region Campaign CRUD

        IEnumerable<Campaign> IPrepOpsDataAccess.Campaigns
        {
            get
            {
                return _dbContext.Campaigns
                   .Include(x => x.ManagingTenant)
                   .Include(x => x.Activities)
                   .Include(x => x.ParticipatingTenants)
                   .ToList();
            }
        }

        Campaign IPrepOpsDataAccess.GetCampaign(int campaignId)
        {
            return _dbContext.Campaigns
                .Include(x => x.ManagingTenant)
                .Include(x => x.Activities)
                .Include(x => x.ParticipatingTenants)
                .SingleOrDefault(x => x.Id == campaignId);
        }

        Task IPrepOpsDataAccess.AddCampaign(Campaign value)
        {
            _dbContext.Campaigns.Add(value);
            return _dbContext.SaveChangesAsync();
        }

        Task IPrepOpsDataAccess.DeleteCampaign(int id)
        {
            var toDelete = _dbContext.Campaigns.Where(c => c.Id == id).SingleOrDefault();

            if (toDelete != null)
            {
                _dbContext.Campaigns.Remove(toDelete);
                return _dbContext.SaveChangesAsync();
            }
            return null;
        }

        Task IPrepOpsDataAccess.UpdateCampaign(Campaign value)
        {
            _dbContext.Campaigns.Update(value);
            return _dbContext.SaveChangesAsync();
        }

        #endregion

        #region Tenant CRUD
        Tenant IPrepOpsDataAccess.GetTenant(int tenantId)
        {
            return _dbContext.Tenants.Include(t => t.Campaigns).SingleOrDefault(t => t.Id == tenantId);
        }

        Task IPrepOpsDataAccess.AddTenant(Tenant value)
        {
            _dbContext.Tenants.Add(value);
            return _dbContext.SaveChangesAsync();
        }

        Task IPrepOpsDataAccess.DeleteTenant(int id)
        {
            var tenant = _dbContext.Tenants.SingleOrDefault(c => c.Id == id);

            if (tenant != null)
            {
                _dbContext.Tenants.Remove(tenant);
                return _dbContext.SaveChangesAsync();
            }
            return null;
        }

        Task IPrepOpsDataAccess.UpdateTenant(Tenant value)
        {
            var tenant = _dbContext.Tenants.SingleOrDefault(c => c.Id == value.Id);

            if (tenant != null)
            {
                _dbContext.Tenants.Update(tenant);
                return _dbContext.SaveChangesAsync();
            }
            return null;
        }

        IEnumerable<Tenant> IPrepOpsDataAccess.Tenants
        {
            get
            {
                return _dbContext.Tenants.Include(x => x.Campaigns).ToList();
            }
        }
        #endregion

        #region ApplicationUser CRUD

        IEnumerable<ApplicationUser> IPrepOpsDataAccess.Users
        {
            get
            {
                return _dbContext.Users.Include(u => u.AssociatedTenant).ToList();
            }
        }
        ApplicationUser IPrepOpsDataAccess.GetUser(string userId)
        {
            return _dbContext.Users.Where(u => u.Id == userId).Include(u => u.AssociatedTenant).SingleOrDefault();
        }

        Task IPrepOpsDataAccess.AddUser(ApplicationUser value)
        {
            _dbContext.Users.Add(value);
            return _dbContext.SaveChangesAsync();
        }

        Task IPrepOpsDataAccess.DeleteUser(string userId)
        {
            var toDelete = _dbContext.Users.Where(u => u.Id == userId).SingleOrDefault();
            if (toDelete != null)
            {
                _dbContext.Users.Remove(toDelete);
                return _dbContext.SaveChangesAsync();
            }
            return null;
        }

        Task IPrepOpsDataAccess.UpdateUser(ApplicationUser value)
        {
            _dbContext.Users.Update(value);
            return _dbContext.SaveChangesAsync();
        }

        #endregion

        #region ActivitySignup CRUD

        IEnumerable<ActivitySignup> IPrepOpsDataAccess.ActivitySignups
        {
            get
            {
                return _dbContext.ActivitySignup
                        .Include(z => z.User)
                        .Include(x => x.Activity)
                        .Include(x => x.Activity.UsersSignedUp)
                        .ThenInclude(u => u.User)
                        .ToList();
            }
        }

        ActivitySignup IPrepOpsDataAccess.GetActivitySignup(int id, string userId)
        {
            return _dbContext.ActivitySignup
                .Include(z => z.User)
                .Include(x => x.Activity)
                .Include(x => x.Activity.UsersSignedUp)
                .ToArray()
                .Where(x => x.Activity.Id == id)
                .Where(x => x.User.Id == userId)
                .SingleOrDefault();
        }

        Task IPrepOpsDataAccess.AddActivitySignup(ActivitySignup userSignup)
        {
            _dbContext.ActivitySignup.Add(userSignup);
            return _dbContext.SaveChangesAsync();
        }

        Task IPrepOpsDataAccess.DeleteActivitySignup(int activitySignupId)
        {
            var activity = _dbContext.ActivitySignup.SingleOrDefault(c => c.Id == activitySignupId);

            if (activity != null)
            {
                _dbContext.ActivitySignup.Remove(activity);
                return _dbContext.SaveChangesAsync();
            }
            return null;
        }

        Task IPrepOpsDataAccess.UpdateActivitySignup(ActivitySignup value)
        {
            _dbContext.ActivitySignup.Update(value);
            return _dbContext.SaveChangesAsync();
        }

        #endregion

        #region TaskSignup CRUD
        IEnumerable<TaskUsers> IPrepOpsDataAccess.TaskSignups
        {
            get
            {
                return _dbContext.TaskSignup
                          .Include(x => x.Task)
                          .Include(x => x.User)
                          .Include(x => x.Task.Activity)
                          .Include(x => x.Task.Activity.Campaign)
                          .Include(x => x.Task.Tenant)
                          .ToArray();
            }
        }

        TaskUsers IPrepOpsDataAccess.GetTaskSignup(int taskId, string userId)
        {
            return _dbContext.TaskSignup
              .Include(x => x.Task)
              .Include(x => x.User)
              .ToArray()
              .Where(x => x.Task.Id == taskId)
              .Where(x => x.User.Id.Equals(userId))
              .SingleOrDefault();
        }

        Task IPrepOpsDataAccess.AddTaskSignup(TaskUsers taskSignup)
        {
            _dbContext.TaskSignup.Add(taskSignup);
            return _dbContext.SaveChangesAsync();
        }

        Task IPrepOpsDataAccess.DeleteTaskSignup(int taskSignupId)
        {
            var taskSignup = _dbContext.TaskSignup.SingleOrDefault(c => c.Id == taskSignupId);

            if (taskSignup != null)
            {
                _dbContext.TaskSignup.Remove(taskSignup);
                return _dbContext.SaveChangesAsync();
            }
            return null;
        }

        
        Task IPrepOpsDataAccess.UpdateTaskSignup(TaskUsers value)
        {
            _dbContext.TaskSignup.Update(value);
            return _dbContext.SaveChangesAsync();
        }


        #endregion

        #region Task CRUD


        IEnumerable<PrepOpsTask> IPrepOpsDataAccess.Tasks
        {
            get
            {
                return _dbContext.Tasks
                  .Include(x => x.Tenant)
                  .Include(x => x.Activity)
                  .Include(x => x.Activity.Campaign)
                  .Include(x => x.AssignedVolunteers)
                  .ToList();
            }
        }

        PrepOpsTask IPrepOpsDataAccess.GetTask(int taskId, string userId)
        {
            var taskUser = _dbContext.TaskSignup.Where(tu => tu.User.Id == userId).SingleOrDefault();

            return taskUser == null ? null :
                _dbContext.Tasks
                  .Include(x => x.Tenant)
                  .Include(x => x.Activity)
                  .Include(x => x.Activity.Campaign)
                  .Include(x => x.AssignedVolunteers)
                  .Where(t => t.Id == taskId && t.AssignedVolunteers.Contains(taskUser)).SingleOrDefault();
        }

        PrepOpsTask IPrepOpsDataAccess.GetTask(int taskId)
        {
            return _dbContext.Tasks
                .Include(x => x.Tenant)
                .Include(x => x.Activity)
                .Include(x => x.Activity.Campaign)
                .Include(x => x.AssignedVolunteers)
                .Where(t => t.Id == taskId).SingleOrDefault();
        }

        Task IPrepOpsDataAccess.AddTask(PrepOpsTask task)
        {
            if (task.Id == 0)
            {
                _dbContext.Add(task);
                return _dbContext.SaveChangesAsync();
            }
            else throw new InvalidOperationException("Added task that already has Id");
        }

        Task IPrepOpsDataAccess.DeleteTask(int taskId)
        {
            var toDelete = _dbContext.Tasks.Where(t => t.Id == taskId).SingleOrDefault();

            if (toDelete != null)
            {
                _dbContext.Tasks.Remove(toDelete);
                return _dbContext.SaveChangesAsync();
            }
            return null;
        }

        Task IPrepOpsDataAccess.UpdateTask(PrepOpsTask value)
        {
            _dbContext.Tasks.Update(value);
            return _dbContext.SaveChangesAsync();
        }
        #endregion
    }
}
