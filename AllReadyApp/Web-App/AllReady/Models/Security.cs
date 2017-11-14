namespace AllReady.Models
{
    public enum UserType
    {
        BasicUser,
        OrgAdmin,
        SiteAdmin,
        ApiAccess
    }
    public class TokenTypes
    {
        public const string ApiKey = "api-key";
    }
}
