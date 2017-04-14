using System;
using System.Threading.Tasks;

namespace AllReady.Security
{
    /// <summary>
    /// Builder for <see cref="IAuthorizableRequest"/>s
    /// </summary>
    public interface IAuthorizableRequestBuilder
    {
        /// <summary>
        /// Builds an instance of an <see cref="IAuthorizableRequest"/>
        /// </summary>
        Task<IAuthorizableRequest> Build(Guid requestId, int? itineraryId = null, int? eventId = null, int? campaignId = null, int? orgId = null);
    }
}
