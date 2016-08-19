using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AllReady.Models
{
    /// <summary>
    /// We are moving away from the use of IAllReadyDataAccess and to the command/query/handler
    /// pattern. For a sample implemenation, look in ~/Areas/Admin/Features or ~/Features for 
    /// reference classes you can use. If you are able to reduce usage of any method here to a 
    /// count of 1 (meaning it would only exist in the implemenation) it is safe to remove the 
    /// interface signature and the concrete implementation from the corresponding 
    /// AllReadyDataAccess partial class.
    /// </summary>


    [Obsolete("Please favor commands/queries; see https://github.com/HTBox/allReady/issues/1130", false)]
    public interface IAllReadyDataAccess
    {

        #region Event CRUD
        [Obsolete("Please favor commands/queries; see https://github.com/HTBox/allReady/issues/1130", false)]
        IEnumerable<Event> Events { get; }
        [Obsolete("Please favor commands/queries; see https://github.com/HTBox/allReady/issues/1130", false)]
        Event GetEvent(int eventId);
        [Obsolete("Please favor commands/queries; see https://github.com/HTBox/allReady/issues/1130", false)]
        int GetManagingOrganizationId(int eventId);
        [Obsolete("Please favor commands/queries; see https://github.com/HTBox/allReady/issues/1130", false)]
        IEnumerable<EventSignup> GetEventSignups(string userId);
        [Obsolete("Please favor commands/queries; see https://github.com/HTBox/allReady/issues/1130", false)]
        IEnumerable<EventSignup> GetEventSignups(int eventId, string userId);
        [Obsolete("Please favor commands/queries; see https://github.com/HTBox/allReady/issues/1130", false)]
        Task UpdateEvent(Event value);
        [Obsolete("Please favor commands/queries; see https://github.com/HTBox/allReady/issues/1130", false)]
        IEnumerable<Event> EventsByPostalCode(string postalCode, int distance);
        [Obsolete("Please favor commands/queries; see https://github.com/HTBox/allReady/issues/1130", false)]
        IEnumerable<Event> EventsByGeography(double latitude, double longitude, int distance);
        [Obsolete("Please favor commands/queries; see https://github.com/HTBox/allReady/issues/1130", false)]
        IEnumerable<Resource> GetResourcesByCategory(string category);

        #endregion

        #region Campaign CRUD

        [Obsolete("Please favor commands/queries; see https://github.com/HTBox/allReady/issues/1130", false)]
        IEnumerable<Campaign> Campaigns { get; }
        [Obsolete("Please favor commands/queries; see https://github.com/HTBox/allReady/issues/1130", false)]
        Campaign GetCampaign(int campaignId);

        #endregion

        #region User CRUD
        [Obsolete("Please favor commands/queries; see https://github.com/HTBox/allReady/issues/1130", false)]
        ApplicationUser GetUser(string userId);

        [Obsolete("Please favor commands/queries; see https://github.com/HTBox/allReady/issues/1130", false)]
        Task UpdateUser(ApplicationUser value);
        #endregion

        #region EventSignup CRUD

        [Obsolete("Please favor commands/queries; see https://github.com/HTBox/allReady/issues/1130", false)]
        IEnumerable<EventSignup> EventSignups { get; }

        [Obsolete("Please favor commands/queries; see https://github.com/HTBox/allReady/issues/1130", false)]
        EventSignup GetEventSignup(int eventId, string userId);

        [Obsolete("Please favor commands/queries; see https://github.com/HTBox/allReady/issues/1130", false)]
        Task AddEventSignupAsync(EventSignup userSignup);

        [Obsolete("Please favor commands/queries; see https://github.com/HTBox/allReady/issues/1130", false)]
        Task DeleteEventAndTaskSignupsAsync(int eventSignupId);

        #endregion

        #region TaskSignup CRUD
        [Obsolete("Please favor commands/queries; see https://github.com/HTBox/allReady/issues/1130", false)]
        IEnumerable<TaskSignup> TaskSignups { get; }

        [Obsolete("Please favor commands/queries; see https://github.com/HTBox/allReady/issues/1130", false)]
        Task UpdateTaskSignupAsync(TaskSignup value);
        #endregion

        #region AllReadyTask CRUD
        [Obsolete("Please favor commands/queries; see https://github.com/HTBox/allReady/issues/1130", false)]
        IEnumerable<AllReadyTask> Tasks { get; }

        [Obsolete("Please favor commands/queries; see https://github.com/HTBox/allReady/issues/1130", false)]
        AllReadyTask GetTask(int taskId);

        [Obsolete("Please favor commands/queries; see https://github.com/HTBox/allReady/issues/1130", false)]
        Task AddTaskAsync(AllReadyTask task);

        [Obsolete("Please favor commands/queries; see https://github.com/HTBox/allReady/issues/1130", false)]
        Task DeleteTaskAsync(int taskId);

        [Obsolete("Please favor commands/queries; see https://github.com/HTBox/allReady/issues/1130", false)]
        Task UpdateTaskAsync(AllReadyTask value);

        [Obsolete("Please favor commands/queries; see https://github.com/HTBox/allReady/issues/1130", false)]
        IEnumerable<TaskSignup> GetTasksAssignedToUser(int eventId, string userId);

        #endregion

        #region Request CRUD
        [Obsolete("Please favor commands/queries; see https://github.com/HTBox/allReady/issues/1130", false)]
        Task<Request> GetRequestByProviderIdAsync( string providerId );

        [Obsolete("Please favor commands/queries; see https://github.com/HTBox/allReady/issues/1130", false)]
        Task AddRequestAsync( Request request ); 
        #endregion

    }
}
