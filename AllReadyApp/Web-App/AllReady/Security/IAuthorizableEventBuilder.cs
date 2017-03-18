using System.Threading.Tasks;

namespace AllReady.Security
{
    /// <summary>
    /// Builder for <see cref="IAuthorizableEvent"/>s
    /// </summary>
    public interface IAuthorizableEventBuilder
    {
        /// <summary>
        /// Builds an instance of an <see cref="IAuthorizableEvent"/>
        /// </summary>
        Task<IAuthorizableEvent> Build(int eventId, int? campaignId = null, int? orgId = null);
    }
}