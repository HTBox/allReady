using Microsoft.AspNetCore.Mvc;
using System;

namespace AllReady.Controllers
{
    public static class ControllerExtensions
    {
        public static string Name(this Controller controller)
        {
            return Constants.ControllerNames.NameOf(controller.GetType());
        }

        public static string Name(this Controller _, Type otherConrrollerType)
        {
            return Constants.ControllerNames.NameOf(otherConrrollerType);
        }
    }
}
