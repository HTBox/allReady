using System;
using System.Collections.Generic;
using System.Text;

namespace AllReady.BrowserTest
{
    public class User
    {
        public enum Role
        {
            AllReadyAdministrator,
            OrganizationAdministrator,
            User
        }

        public string Name { get; private set; }
        public string Password { get; private set; }

        public User(Role role)
        {
            var config = new ConfigurationFixture().Config;

            var prefix = role.ToString();

            Name = config[$"{prefix}UserEmail"];
            Password = config[$"{prefix}Password"];
        }
    }
}
