using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using System.Security.Claims;

namespace AllReady.Extensions
{
    public static class ClaimsExtensions
    {
        public static bool IsUserType(this IList<Claim> claims, UserType type)
        {
            bool isAdmin = false;
            
            IList<Claim> claimValues = claims.Where(c => c.Type.Equals(typeof(UserType).Name)).ToList();
            string userTypeString = Enum.GetName(typeof(UserType), type);
            foreach (var c in claimValues)
            {
                if (c.Value.Equals(userTypeString))
                {
                    isAdmin = true;
                    break;
                }
            }
            return isAdmin;
        }
    }
}
