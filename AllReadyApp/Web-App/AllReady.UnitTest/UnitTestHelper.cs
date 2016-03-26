using System.Security.Claims;

namespace AllReady.UnitTest
{
    public class UnitTestHelper
    {
        public static ClaimsPrincipal GetClaimsPrincipal(string userType, int organizationId)
        {
            return new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim(AllReady.Security.ClaimTypes.UserType, userType),
                        new Claim(AllReady.Security.ClaimTypes.Organization, organizationId.ToString()),
                    }));
        }
    }
}