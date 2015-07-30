using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Models
{

    public enum TenantSecurity
    {
        Volunteer = 1,
        Employee = 2,
        Admin = 3
    }

    public enum UserType
    {
        BasicUser,
        TenantAdmin,
        SiteAdmin
    }
}
