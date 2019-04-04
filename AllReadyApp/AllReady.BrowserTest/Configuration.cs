using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AllReady.BrowserTest
{
    static public class Configuration
    {
        //static public string HomePageURL = @"http://localhost:48408";
        //static public string AllReadyAdministratorUserEmail = @"Administrator@example.com";
        //static public string AllReadyAdministratorPassword = @"YouShouldChangeThisPassword1!";
        //static public string OrganizationAdministratorUserEmail = @"organization@example.com";
        //static public string OrganizationAdministratorPassword = @"YouShouldChangeThisPassword1!";

        static IConfiguration config = null;

        static IConfiguration Instance()
        {
            if (config == null)
            {
                var builder = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json");

                config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", true, true)
                    .Build();
            }

            return config;
        }

        static public string HomePageURL { get { return Instance()["HomePageURL"]; } }
        static public string AllReadyAdministratorUserEmail { get { return Instance()["AllReadyAdministratorUserEmail"]; } }
        static public string AllReadyAdministratorPassword { get { return Instance()["AllReadyAdministratorPassword"]; } }
        static public string OrganizationAdministratorUserEmail { get { return Instance()["OrganizationAdministratorUserEmail"]; } }
        static public string OrganizationAdministratorPassword { get { return Instance()["OrganizationAdministratorPassword"]; } }
    }

}
