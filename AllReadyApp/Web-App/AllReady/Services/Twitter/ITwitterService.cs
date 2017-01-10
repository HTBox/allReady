using System.Threading.Tasks;

namespace AllReady.Services.Twitter
{
    public interface ITwitterService
    {
        Task<TwitterUserInfo> GetTwitterAccount(string userId, string screenName);
    }
}
