using System.Collections.Generic;
using System.Reflection;

using AllReady.Areas.Admin.ViewModels.Validators;
using AllReady.Areas.Admin.ViewModels.Validators.VolunteerTask;
using AllReady.Controllers;
using AllReady.DataAccess;
using AllReady.Providers.ExternalUserInformationProviders;
using AllReady.Providers.ExternalUserInformationProviders.Providers;
using AllReady.Services;
using AllReady.Services.Mapping.GeoCoding;
using AllReady.Services.Mapping.Routing;
using AllReady.Services.Sms;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Features.Variance;
using CsvHelper;
using Hangfire;
using Hangfire.SqlServer;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AllReady.Configuration
{
    internal static class Services
    {
        internal static IContainer CreateIoCContainer(IServiceCollection services, IConfiguration configuration)
        {
            // todo: move these to a proper autofac module
            // Register application services.
            services.AddSingleton(x => configuration);
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddTransient<IUrlHelper>(x => new UrlHelper(x.GetService<IActionContextAccessor>().ActionContext));
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddTransient<IDetermineIfATaskIsEditable, DetermineIfATaskIsEditable>();
            services.AddTransient<IValidateEventEditViewModels, EventEditViewModelValidator>();
            services.AddTransient<IValidateVolunteerTaskEditViewModelValidator, VolunteerTaskEditViewModelValidator>();
            services.AddTransient<IItineraryEditModelValidator, ItineraryEditModelValidator>();
            services.AddTransient<IOrganizationEditModelValidator, OrganizationEditModelValidator>();
            services.AddTransient<IRedirectAccountControllerRequests, RedirectAccountControllerRequests>();
            services.AddSingleton<ICsvFactory, CsvFactory>();
            services.AddTransient<SampleDataGenerator>();
            services.AddSingleton<IHttpClient, StaticHttpClient>();
            services.AddTransient<IBlockBlob, BlockBlob>();
            services.AddTransient<IAttachmentService, AttachmentService>();
            services.AddSingleton<IImageSizeValidator, ImageSizeValidator>();

            if (configuration["Mapping:EnableGoogleGeocodingService"] == "true")
            {
                services.AddSingleton<IGeocodeService, GoogleGeocodeService>();
            }
            else
            {
                services.AddSingleton<IGeocodeService, FakeGeocodeService>();
            }

            if (configuration["Data:Storage:EnableAzureBlobImageService"] == "true")
            {
                services.AddSingleton<IImageService, ImageService>();
            }
            else
            {
                services.AddSingleton<IImageService, FakeImageService>();
            }

            if (configuration["Data:Storage:EnableAzureQueueService"] == "true")
            {
                // This setting is false by default. To enable queue processing you will
                // need to override the setting in your user secrets or env vars.
                services.AddTransient<IQueueStorageService, QueueStorageService>();
            }
            else
            {
                // this writer service will just write to the default logger
                services.AddTransient<IQueueStorageService, FakeQueueWriterService>();
            }

            if (configuration["Authentication:Twilio:EnableTwilio"] == "true")
            {
                services.AddSingleton<IPhoneNumberLookupService, TwilioPhoneNumberLookupService>();
                services.AddSingleton<ITwilioWrapper, TwilioWrapper>();
            }
            else
            {
                services.AddSingleton<IPhoneNumberLookupService, FakePhoneNumberLookupService>();
            }

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterSource(new ContravariantRegistrationSource());
            containerBuilder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly).AsImplementedInterfaces();
            containerBuilder.RegisterAssemblyTypes(typeof(Startup).GetTypeInfo().Assembly)
                .Where(t => !t.IsAssignableTo<TagHelper>()).AsImplementedInterfaces();

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

            //ExternalUserInformationProviderFactory registration
            containerBuilder.RegisterType<TwitterExternalUserInformationProvider>().Named<IProvideExternalUserInformation>("Twitter");
            containerBuilder.RegisterType<GoogleExternalUserInformationProvider>().Named<IProvideExternalUserInformation>("Google");
            containerBuilder.RegisterType<MicrosoftAndFacebookExternalUserInformationProvider>().Named<IProvideExternalUserInformation>("Microsoft");
            containerBuilder.RegisterType<MicrosoftAndFacebookExternalUserInformationProvider>().Named<IProvideExternalUserInformation>("Facebook");
            containerBuilder.RegisterType<ExternalUserInformationProviderFactory>().As<IExternalUserInformationProviderFactory>();

            //Hangfire
            containerBuilder.Register(icomponentcontext => new BackgroundJobClient(new SqlServerStorage(configuration["Data:HangfireConnection:ConnectionString"])))
                .As<IBackgroundJobClient>();

            //auto-register Hangfire jobs by convention
            //http://docs.autofac.org/en/latest/register/scanning.html
            var assembly = typeof(Startup).GetTypeInfo().Assembly;
            containerBuilder
                .RegisterAssemblyTypes(assembly)
                .Where(t => t.Namespace == "AllReady.Hangfire.Jobs" && t.GetTypeInfo().IsInterface)
                .AsImplementedInterfaces();

            containerBuilder.RegisterType<GoogleOptimizeRouteService>().As<IOptimizeRouteService>().SingleInstance();

            //Populate the container with services that were previously registered
            containerBuilder.Populate(services);

            var container = containerBuilder.Build();
            return container;
        }
    }
}
