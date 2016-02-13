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
        int GetManagingOrganizationId(int activityId);
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
        Task UpdateCampaign(Campaign value);

        #endregion

        #region Organization CRUD

        IEnumerable<Organization> Organziations { get; }
        Organization GetOrganization(int organizationId);
        Task AddOrganization(Organization value);
        Task DeleteOrganization(int id);
        Task UpdateOrganization(Organization value);

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

        Task DeleteActivityAndTaskSignupsAsync(int activitySignupId);

        Task UpdateActivitySignupAsync(ActivitySignup value);

        #endregion

        #region TaskSignup CRUD
        IEnumerable<TaskSignup> TaskSignups { get; }

        TaskSignup GetTaskSignup(int taskId, string userId);

        Task AddTaskSignupAsync(TaskSignup taskSignup);

        Task DeleteTaskSignupAsync(int taskSignupId);

        Task UpdateTaskSignupAsync(TaskSignup value);
        #endregion

        #region AllReadyTask CRUD
        IEnumerable<AllReadyTask> Tasks { get; }

        AllReadyTask GetTask(int taskId, string userId);

        AllReadyTask GetTask(int taskId);

        Task AddTaskAsync(AllReadyTask task);

        Task DeleteTaskAsync(int taskId);

        Task UpdateTaskAsync(AllReadyTask value);
        IEnumerable<TaskSignup> GetTasksAssignedToUser(int activityId, string userId);

        #endregion

        #region Skill CRUD
        IEnumerable<Skill> Skills { get; }

        #endregion

        #region Closest Locations

        IEnumerable<ClosestLocation> GetClosestLocations(LocationQuery query);
        IEnumerable<PostalCodeGeoCoordinate> GetPostalCodeCoordinates(string postalCode);

        #endregion Closest Locations
    }
}
