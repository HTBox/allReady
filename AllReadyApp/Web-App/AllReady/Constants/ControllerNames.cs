using System;

namespace AllReady.Constants
{
    public static class ControllerNames
    {
        public const string Admin = "Admin";
        private const string ControllerSuffix = "Controller";

        /// <summary>
        /// Use to take the Controller name without the "Controller" suffix. For example
        /// NameOf(DashboarHubController) == "DashboardHub".
        /// Useful for the url redirect use case and similar. 
        /// </summary>
        /// <param name="controllerType"></param>
        /// <returns></returns>
        public static string NameOf(Type controllerType)
        {
            if (controllerType == null)
            {
                throw new ArgumentNullException(nameof(controllerType), "Passed in null type. Expected controller type.");
            }

            return controllerType.Name.Replace(ControllerSuffix, string.Empty);     
        }
    }
}
