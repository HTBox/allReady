using AllReady.Areas.Admin.Controllers;
using AllReady.Models;
using AllReady.UnitTest.Areas.Admin.Controllers;
using AllReady.UnitTest.Extensions;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Features.Variance;
using MediatR;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Http.Features.Authentication;
using Microsoft.AspNet.Http.Features.Authentication.Internal;
using Microsoft.AspNet.Http.Internal;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Edm.Validation;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest
{
    public class ContextFixture : IDisposable
    {
        public AllReadyContext Context { get; private set; }
        public UserManager<ApplicationUser> UserManager { get; }
        public IServiceProvider ServiceProvider { get; private set; }

        public ContextFixture()
        {
            var services = new ServiceCollection();

            // set up empty in-memory test db
            services.AddEntityFramework()
                .AddInMemoryDatabase()
                .AddDbContext<AllReadyContext>(options => options.UseInMemoryDatabase());

            // add identity service
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AllReadyContext>();
            var context = new DefaultHttpContext();
            context.Features.Set<IHttpAuthenticationFeature>(new HttpAuthenticationFeature());
            services.AddSingleton<IHttpContextAccessor>(h => new HttpContextAccessor { HttpContext = context });

            // Setup hosting environment
            IHostingEnvironment hostingEnvironment = new HostingEnvironment();
            hostingEnvironment.EnvironmentName = "Development";
            services.AddSingleton(x => hostingEnvironment);

            // set up service provider for tests
            ServiceProvider = services.BuildServiceProvider();

            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterSource(new ContravariantRegistrationSource());
            containerBuilder.RegisterAssemblyTypes(typeof(IMediator).Assembly).AsImplementedInterfaces();
            containerBuilder.RegisterAssemblyTypes(typeof(Startup).Assembly).AsImplementedInterfaces();
            containerBuilder.Register<SingleInstanceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });

            containerBuilder.Register<MultiInstanceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => (IEnumerable<object>)c.Resolve(typeof(IEnumerable<>).MakeGenericType(t));
            });

            //Populate the container with services that were previously registered
            containerBuilder.Populate(services);

            var container = containerBuilder.Build();
            this.ServiceProvider = container.Resolve<IServiceProvider>();

            this.Context = ServiceProvider.GetService<AllReadyContext>();
            this.UserManager = ServiceProvider.GetService<UserManager<ApplicationUser>>();
        }
        public void EmptyDatabase()
        {
            foreach (EntityEntry entity in Context.ChangeTracker.Entries().ToList())
            {
                if (entity.Entity != null) Context.Remove(entity.Entity);
            }
            Context.SaveChanges();

        }
        public void Dispose()
        {
            this.Context.Dispose();
            this.UserManager.Dispose();
        }
    }

    [CollectionDefinition("Context Collection")]
    public class ContextCollection : ICollectionFixture<ContextFixture>
    {
    }

}
