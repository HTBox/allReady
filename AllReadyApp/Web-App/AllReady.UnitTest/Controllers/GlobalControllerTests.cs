using AllReady.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Reflection;
using AllReady.Attributes;
using Xunit;

namespace AllReady.UnitTest.Controllers
{
    public class GlobalControllerTests
    {
        /// <summary>
        /// A test to reflect across all controllers and valide all post requests also have the ValidateAntiForgeryToken attribute
        /// </summary>
        /// <remarks>Code credit - http://codiply.com/blog/test-for-omission-of-validateantiforgerytoken-attribute-in-asp-net-mvc"</remarks>
        [Fact]
        public void AllHttpPostControllerActionsShouldBeDecoratedWithValidateAntiForgeryTokenAttribute()
        {
            var allControllerTypes =
                typeof(AccountController).GetTypeInfo().Assembly.GetTypes()
                .Where(type => typeof(Controller).IsAssignableFrom(type));
            var allControllerActions = allControllerTypes.SelectMany(type => type.GetMethods());

            var failingActions = allControllerActions
                .Where(method => method.GetType().GetTypeInfo().GetCustomAttribute<HttpPostAttribute>() != null)
                .Where(method => method.GetType().GetTypeInfo().GetCustomAttribute<ExternalEndpointAttribute>() != null)
                .Where(method => method.GetType().GetTypeInfo().GetCustomAttribute<ValidateAntiForgeryTokenAttribute>() == null)
                .ToList();

            var message = string.Empty;
            if (failingActions.Any())
            {
                message =
                    failingActions.Count() + " failing action" +
                    (failingActions.Count() == 1 ? ":\n" : "s:\n") +
                    failingActions.Select(method => method.Name + " in " + method.DeclaringType.Name)
                        .Aggregate((a, b) => a + ",\n" + b);
            }

            Assert.False(failingActions.Any(), message);
        }
    }
}