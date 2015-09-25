using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Relational;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.InMemory;

namespace AllReady.Models
{
  public class AllReadyDataAccessEF7 : IAllReadyDataAccess
  {
    #region Fields, Constructors, Context setup

    private readonly AllReadyContext _dbContext;

    public AllReadyDataAccessEF7(AllReadyContext dbContext)
    {
      _dbContext = dbContext;
    }

    #endregion

    #region Activity CRUD
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
    #endregion

    #region Campaign CRUD

    IEnumerable<Campaign> IAllReadyDataAccess.Campaigns
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

    Campaign IAllReadyDataAccess.GetCampaign(int campaignId)
    {
      return _dbContext.Campaigns
          .Include(x => x.ManagingTenant)
          .Include(x => x.Activities)
          .Include(x => x.ParticipatingTenants)
          .SingleOrDefault(x => x.Id == campaignId);
    }

    Task IAllReadyDataAccess.AddCampaign(Campaign value)
    {
      _dbContext.Campaigns.Add(value);
      return _dbContext.SaveChangesAsync();
    }

    Task IAllReadyDataAccess.DeleteCampaign(int id)
    {
      var toDelete = _dbContext.Campaigns.Where(c => c.Id == id).SingleOrDefault();

      if (toDelete != null)
      {
        _dbContext.Campaigns.Remove(toDelete);
        return _dbContext.SaveChangesAsync();
      }
      return null;
    }

    Task IAllReadyDataAccess.UpdateCampaign(Campaign value)
    {
      _dbContext.Campaigns.Update(value);
      return _dbContext.SaveChangesAsync();
    }

    #endregion

    #region Tenant CRUD
    Tenant IAllReadyDataAccess.GetTenant(int tenantId)
    {
      return _dbContext.Tenants.Include(t => t.Campaigns).SingleOrDefault(t => t.Id == tenantId);
    }

    Task IAllReadyDataAccess.AddTenant(Tenant value)
    {
      _dbContext.Tenants.Add(value);
      return _dbContext.SaveChangesAsync();
    }

    Task IAllReadyDataAccess.DeleteTenant(int id)
    {
      var tenant = _dbContext.Tenants.SingleOrDefault(c => c.Id == id);

      if (tenant != null)
      {
        _dbContext.Tenants.Remove(tenant);
        return _dbContext.SaveChangesAsync();
      }
      return null;
    }

    Task IAllReadyDataAccess.UpdateTenant(Tenant value)
    {
      var tenant = _dbContext.Tenants.SingleOrDefault(c => c.Id == value.Id);

      if (tenant != null)
      {
        _dbContext.Tenants.Update(tenant);
        return _dbContext.SaveChangesAsync();
      }
      return null;
    }

    IEnumerable<Tenant> IAllReadyDataAccess.Tenants
    {
      get
      {
        return _dbContext.Tenants.Include(x => x.Campaigns).ToList();
      }
    }
    #endregion

    #region ApplicationUser CRUD

    IEnumerable<ApplicationUser> IAllReadyDataAccess.Users
    {
      get
      {
        return _dbContext.Users.Include(u => u.AssociatedTenant).ToList();
      }
    }
    ApplicationUser IAllReadyDataAccess.GetUser(string userId)
    {
      return _dbContext.Users.Where(u => u.Id == userId).Include(u => u.AssociatedTenant).SingleOrDefault();
    }

    Task IAllReadyDataAccess.AddUser(ApplicationUser value)
    {
      _dbContext.Users.Add(value);
      return _dbContext.SaveChangesAsync();
    }

    Task IAllReadyDataAccess.DeleteUser(string userId)
    {
      var toDelete = _dbContext.Users.Where(u => u.Id == userId).SingleOrDefault();
      if (toDelete != null)
      {
        _dbContext.Users.Remove(toDelete);
        return _dbContext.SaveChangesAsync();
      }
      return null;
    }

    Task IAllReadyDataAccess.UpdateUser(ApplicationUser value)
    {
      _dbContext.Users.Update(value);
      return _dbContext.SaveChangesAsync();
    }

    #endregion

    #region ActivitySignup CRUD

    IEnumerable<ActivitySignup> IAllReadyDataAccess.ActivitySignups
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

    ActivitySignup IAllReadyDataAccess.GetActivitySignup(int id, string userId)
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

    Task IAllReadyDataAccess.AddActivitySignupAsync(ActivitySignup userSignup)
    {
      _dbContext.ActivitySignup.Add(userSignup);
      return _dbContext.SaveChangesAsync();
    }

    Task IAllReadyDataAccess.DeleteActivitySignupAsync(int activitySignupId)
    {
      var activity = _dbContext.ActivitySignup.SingleOrDefault(c => c.Id == activitySignupId);

      if (activity != null)
      {
        _dbContext.ActivitySignup.Remove(activity);
        return _dbContext.SaveChangesAsync();
      }
      return null;
    }

    Task IAllReadyDataAccess.UpdateActivitySignupAsync(ActivitySignup value)
    {
      _dbContext.ActivitySignup.Update(value);
      return _dbContext.SaveChangesAsync();
    }

    #endregion

    #region TaskSignup CRUD
    IEnumerable<TaskUsers> IAllReadyDataAccess.TaskSignups
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

    TaskUsers IAllReadyDataAccess.GetTaskSignup(int taskId, string userId)
    {
      return _dbContext.TaskSignup
        .Include(x => x.Task)
        .Include(x => x.User)
        .ToArray()
        .Where(x => x.Task.Id == taskId)
        .Where(x => x.User.Id.Equals(userId))
        .SingleOrDefault();
    }

    Task IAllReadyDataAccess.AddTaskSignupAsync(TaskUsers taskSignup)
    {
      _dbContext.TaskSignup.Add(taskSignup);
      return _dbContext.SaveChangesAsync();
    }

    Task IAllReadyDataAccess.DeleteTaskSignupAsync(int taskSignupId)
    {
      var taskSignup = _dbContext.TaskSignup.SingleOrDefault(c => c.Id == taskSignupId);

      if (taskSignup != null)
      {
        _dbContext.TaskSignup.Remove(taskSignup);
        return _dbContext.SaveChangesAsync();
      }
      return null;
    }


    Task IAllReadyDataAccess.UpdateTaskSignupAsync(TaskUsers value)
    {
      _dbContext.TaskSignup.Update(value);
      return _dbContext.SaveChangesAsync();
    }


    #endregion

    #region Task CRUD


    IEnumerable<AllReadyTask> IAllReadyDataAccess.Tasks
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

    AllReadyTask IAllReadyDataAccess.GetTask(int taskId, string userId)
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

    AllReadyTask IAllReadyDataAccess.GetTask(int taskId)
    {
      return _dbContext.Tasks
          .Include(x => x.Tenant)
          .Include(x => x.Activity)
          .Include(x => x.Activity.Campaign)
          .Include(x => x.AssignedVolunteers)
          .Where(t => t.Id == taskId).SingleOrDefault();
    }

    Task IAllReadyDataAccess.AddTaskAsync(AllReadyTask task)
    {
      if (task.Id == 0)
      {
        _dbContext.Add(task);
        return _dbContext.SaveChangesAsync();
      }
      else throw new InvalidOperationException("Added task that already has Id");
    }

    Task IAllReadyDataAccess.DeleteTaskAsync(int taskId)
    {
      var toDelete = _dbContext.Tasks.Where(t => t.Id == taskId).SingleOrDefault();

      if (toDelete != null)
      {
        _dbContext.Tasks.Remove(toDelete);
        return _dbContext.SaveChangesAsync();
      }
      return null;
    }

    Task IAllReadyDataAccess.UpdateTaskAsync(AllReadyTask value)
    {
      _dbContext.Tasks.Update(value);
      return _dbContext.SaveChangesAsync();
    }
    #endregion
  }
}
