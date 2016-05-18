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

        #region Organization CRUD

        IEnumerable<Organization> Organizations { get; }
        Organization GetOrganization(int organizationId);
        Task AddOrganization(Organization value);
        Task DeleteOrganization(int id);
        Task UpdateOrganization(Organization value);

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

        Task DeleteEventAndTaskSignupsAsync(int eventSignupId);

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

        #region Closest Locations

        IEnumerable<ClosestLocation> GetClosestLocations(LocationQuery query);
        IEnumerable<PostalCodeGeoCoordinate> GetPostalCodeCoordinates(string postalCode);

        #endregion Closest Locations
    }
}
