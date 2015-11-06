using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Models
{
    public interface IAllReadyDataAccess
    {

        #region Activity CRUD
        IEnumerable<Activity> Activities { get; }
        Activity GetActivity(int activityId);
        int GetManagingTenantId(int activityId);
        Task AddActivity(Activity value);
        Task DeleteActivity(int id);
        IEnumerable<ActivitySignup> GetActivitySignups(string userId);
        IEnumerable<ActivitySignup> GetActivitySignups(int activityId, string userId);
        Task UpdateActivity(Activity value);
        IEnumerable<Activity> ActivitiesByPostalCode(string postalCode, int distance);
        IEnumerable<Activity> ActivitiesByGeography(double latitude, double longitude, int distance);
        IEnumerable<Resource> GetResourcesByCategory(string category);

        #endregion

        #region Campaign CRUD

        IEnumerable<Campaign> Campaigns { get; }
        Campaign GetCampaign(int campaignId);

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

        Task AddActivitySignupAsync(ActivitySignup userSignup);

        Task DeleteActivitySignupAsync(int activitySignupId);

        Task UpdateActivitySignupAsync(ActivitySignup value);

        #endregion

        #region TaskSignup CRUD
        IEnumerable<TaskUsers> TaskSignups { get; }

        TaskUsers GetTaskSignup(int taskId, string userId);

        Task AddTaskSignupAsync(TaskUsers taskSignup);

        Task DeleteTaskSignupAsync(int taskSignupId);

        Task UpdateTaskSignupAsync(TaskUsers value);
        #endregion

        #region AllReadyTask CRUD
        IEnumerable<AllReadyTask> Tasks { get; }

        AllReadyTask GetTask(int taskId, string userId);

        AllReadyTask GetTask(int taskId);

        Task AddTaskAsync(AllReadyTask task);

        Task DeleteTaskAsync(int taskId);

        Task UpdateTaskAsync(AllReadyTask value);
        IEnumerable<TaskUsers> GetTasksAssignedToUser(int activityId, string userId);

        #endregion

        #region Skill CRUD
        IEnumerable<Skill> Skills { get; }
        Skill GetSkill(int skillId);
        Task AddSkill(Skill value);
        Task DeleteSkill(int id);
        Task UpdateSkill(Skill value);
        #endregion
    }
}
