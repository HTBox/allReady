using Microsoft.AspNet.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Security.Middleware
{
    public class TokenProtectedResourceOptions
    {
        public PathString Path { get; set; }
        public string PolicyName { get; set; }
    }
}
