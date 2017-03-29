using System.Threading.Tasks;

namespace AllReady.Security
{
    /// <summary>
    /// Defines the base requirements for an authorizable domain object such as campaigns or events
    /// </summary>
    public interface IAuthorizable
    {
        /// <summary>
        /// Indicates that the user is authorized to view the <see cref="IAuthorizable"/> object
        /// </summary>
        Task<bool> UserCanView();

        /// <summary>
        /// Indicates that the user is authorized to edit the <see cref="IAuthorizable"/> object
        /// </summary>
        Task<bool> UserCanEdit();

        /// <summary>
        /// Indicates that the user is authorized to delete the <see cref="IAuthorizable"/> object
        /// </summary>
        Task<bool> UserCanDelete();
    }
}