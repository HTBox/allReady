using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Models
{
    public interface IAllReadyDataAccess
    {
        #region Setup and Administration

        void EnsureDatabaseCreated();

        #endregion

        #region Activity CRUD
        IEnumerable<Activity> Activities { get; }
        Activity GetActivity(int activityId);
        Task AddActivity(Activity value);
        Task DeleteActivity(int id);
        IEnumerable<ActivitySignup> GetActivitySignups(ApplicationUser user);
        IEnumerable<ActivitySignup> GetActivitySignups(int activityId, ApplicationUser user);
        Task UpdateActivity(Activity value);
        IEnumerable<Activity> ActivitiesByPostalCode(string postalCode, int distance);
        IEnumerable<Activity> ActivitiesByGeography(double latitude, double longitude, int distance);
        IEnumerable<Resource> GetResourcesByCategory(string category);

        #endregion

        #region Campaign CRUD

        IEnumerable<Campaign> Campaigns { get; }
        Campaign GetCampaign(int campaignId);
        Task AddCampaign(Campaign value);
        Task DeleteCampaign(int id);
        Task UpdateCampaign(Campaign value);

        #endregion

        #region Tenant CRUD

        IEnumerable<Tenant> Tenants { get; }
        Tenant GetTenant(int tenantId);
        Task AddTenant(Tenant value);
        Task DeleteTenant(int id);
        Task UpdateTenant(Tenant value);

        #endregion

        #region User CRUD

        IEnumerable<ApplicationUser> Users { get; }

        ApplicationUser GetUser(string userId);

        Task AddUser(ApplicationUser value);

        Task DeleteUser(string userId);

        Task UpdateUser(ApplicationUser value);

        #endregion

        #region ActivitySignup CRUD

        IEnumerable<ActivitySignup> ActivitySignups { get; }

        ActivitySignup GetActivitySignup(int activityId, string userId);

        Task AddActivitySignup(ActivitySignup userSignup);

        Task DeleteActivitySignup(int activitySignupId);

        Task UpdateActivitySignup(ActivitySignup value);

        #endregion

        #region TaskSignup CRUD
        IEnumerable<TaskUsers> TaskSignups { get; }

        TaskUsers GetTaskSignup(int taskId, string userId);

        Task AddTaskSignup(TaskUsers taskSignup);

        Task DeleteTaskSignup(int taskSignupId);

        Task UpdateTaskSignup(TaskUsers value);
        #endregion

        #region AllReadyTask CRUD
        IEnumerable<AllReadyTask> Tasks { get; }

        AllReadyTask GetTask(int taskId, string userId);

        AllReadyTask GetTask(int taskId);

        Task AddTask(AllReadyTask task);

        Task DeleteTask(int taskId);

        Task UpdateTask(AllReadyTask value);
        IEnumerable<TaskUsers> GetTasksAssignedToUser(int activityId, ApplicationUser user);

        #endregion
    }
}
