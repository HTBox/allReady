using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AllReady.Models
{
    public interface IAllReadyDataAccess
    {

        #region Event CRUD
        IEnumerable<Event> Events { get; }
        Event GetEvent(int eventId);
        int GetManagingOrganizationId(int eventId);
        IEnumerable<EventSignup> GetEventSignups(string userId);
        IEnumerable<EventSignup> GetEventSignups(int eventId, string userId);
        Task UpdateEvent(Event value);
        IEnumerable<Event> EventsByPostalCode(string postalCode, int distance);
        IEnumerable<Event> EventsByGeography(double latitude, double longitude, int distance);
        IEnumerable<Resource> GetResourcesByCategory(string category);

        #endregion

        #region Campaign CRUD

        IEnumerable<Campaign> Campaigns { get; }
        Campaign GetCampaign(int campaignId);

        #endregion

        #region User CRUD

        IEnumerable<ApplicationUser> Users { get; }

        ApplicationUser GetUser(string userId);

        Task UpdateUser(ApplicationUser value);

        #endregion

        #region EventSignup CRUD

        IEnumerable<EventSignup> EventSignups { get; }

        EventSignup GetEventSignup(int eventId, string userId);

        Task AddEventSignupAsync(EventSignup userSignup);

        #endregion

        #region TaskSignup CRUD
        IEnumerable<TaskSignup> TaskSignups { get; }

        Task UpdateTaskSignupAsync(TaskSignup value);
        #endregion

        #region AllReadyTask CRUD
        IEnumerable<AllReadyTask> Tasks { get; }

        AllReadyTask GetTask(int taskId);

        Task AddTaskAsync(AllReadyTask task);

        Task DeleteTaskAsync(int taskId);

        Task UpdateTaskAsync(AllReadyTask value);

        IEnumerable<TaskSignup> GetTasksAssignedToUser(int eventId, string userId);

        #endregion

    }
}
