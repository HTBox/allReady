using System.Threading.Tasks;

namespace AllReady.Security
{
    /// <summary>
    /// Builder for <see cref="IAuthorizableTask"/>s
    /// </summary>
    public interface IAuthorizableTaskBuilder
    {
        /// <summary>
        /// Builds an instance of an <see cref="IAuthorizableTask"/>
        /// </summary>
        Task<IAuthorizableTask> Build(int taskId, int? eventId = null, int? campaignId = null, int? orgId = null);
    }
}
