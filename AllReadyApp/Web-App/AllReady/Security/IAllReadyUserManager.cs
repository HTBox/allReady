using System.Threading.Tasks;
using AllReady.Models;
using Microsoft.AspNetCore.Identity;

namespace AllReady.Security
{
    /// <summary>
    /// Wraps (for testing) the standard UserManager<T> functionality
    /// </summary>
    public interface IAllReadyUserManager
    {
        /// <inheritdoc cref="UserManager{TUser}"/>
        Task<ApplicationUser> FindByEmailAsync(string email);
    }

    public class AllReadyUserManager : IAllReadyUserManager
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AllReadyUserManager(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public Task<ApplicationUser> FindByEmailAsync(string email)
        {
            return _userManager.FindByEmailAsync(email);
        }
    }
}
