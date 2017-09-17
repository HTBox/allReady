using System.Threading.Tasks;

namespace AllReady.Security
{
    /// <summary>
    /// Builder for <see cref="IAuthorizableCampaign"/>s
    /// </summary>
    public interface IAuthorizableCampaignBuilder
    {
        /// <summary>
        /// Builds an instance of an <see cref="IAuthorizableCampaign"/>
        /// </summary>
        Task<IAuthorizableCampaign> Build(int campaignId, int? orgId = null);
    }
}
