namespace AllReady.Services.Twitter
{
    public class TwitterUserInfo
    {
        /// <summary>
        /// name of user
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// User's email-address (null if not filled in on app is
        /// lacking whitelisting)
        /// </summary>
        public string Email { get; set; }
    }
}