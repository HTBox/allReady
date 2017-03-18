using System.Threading.Tasks;

namespace AllReady.Security
{
    /// <summary>
    /// Defines the base requirements for an authorizable domain object such as campaigns or events
    /// </summary>
    public interface IAuthorizable
    {
        /// <summary>
        /// Indicates whether the current user has access to the <see cref="IAuthorizable"/> domain object
        /// </summary>
        /// <returns></returns>
        Task<bool> IsUserAuthorized();
    }
}