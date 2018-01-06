using Microsoft.AspNetCore.Mvc;
using System;

namespace AllReady.Controllers
{
    public static class ControllerExtensions
    {
        /// <summary>
        /// Use to take the Controller name without the "Controller" suffix. For example
        /// NameOf(DashboarHubController) == "DashboardHub".
        /// Useful for the url redirect use case and similar. 
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static string Name(this Controller controller)
        {
            return Constants.ControllerNames.NameOf(controller.GetType());
        }

        /// <summary>
        /// Use to take the Controller name without the "Controller" suffix. For example
        /// NameOf(DashboarHubController) == "DashboardHub".
        /// Useful for the url redirect use case and similar. 
        /// </summary>
        /// <param name="_"></param>
        /// <param name="otherControllerType">The type of the other controller that we want to reference</param>
        /// <returns></returns>
        public static string Name(this Controller _, Type otherControllerType)
        {
            return Constants.ControllerNames.NameOf(otherControllerType);
        }
    }
}
