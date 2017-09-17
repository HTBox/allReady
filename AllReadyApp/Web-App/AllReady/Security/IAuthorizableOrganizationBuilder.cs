using System.Threading.Tasks;

namespace AllReady.Security
{
    /// <summary>
    /// Builder for <see cref="IAuthorizableOrganization"/>s
    /// </summary>
    public interface IAuthorizableOrganizationBuilder
    {
        /// <summary>
        /// Builds an instance of an <see cref="IAuthorizableOrganization"/>
        /// </summary>
        Task<IAuthorizableOrganization> Build(int organizationId);
    }
}
