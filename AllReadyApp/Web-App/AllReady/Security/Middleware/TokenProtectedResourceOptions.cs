using Microsoft.AspNetCore.Http;

namespace AllReady.Security.Middleware
{
    public class TokenProtectedResourceOptions
    {
        public PathString Path { get; set; }
        public string PolicyName { get; set; }
    }
}
