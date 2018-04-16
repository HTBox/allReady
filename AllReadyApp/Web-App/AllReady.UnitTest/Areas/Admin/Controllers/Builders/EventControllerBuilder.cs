using System;
using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.ViewModels.Validators;
using AllReady.Security;
using AllReady.Services;
using MediatR;
using Moq;

namespace AllReady.UnitTest.Areas.Admin.Controllers.Builders
{
    public class EventControllerBuilder
    {
        private IImageService _imageService;
        private IMediator _mediator;
        private IValidateEventEditViewModels _validateEventEditViewModels;
        private IUserAuthorizationService _userAuthorizationService;
        private IImageSizeValidator _imageSizeValidator;

        private Func<DateTime> _dateTimeTodayDate;

        private EventControllerBuilder(IImageService imageService, IMediator mediator, IValidateEventEditViewModels validateEventEditViewModels,
                                        IUserAuthorizationService userAuthorizationService, IImageSizeValidator imageSizeValidator)
        {
            _imageService = imageService;
            _mediator = mediator;
            _validateEventEditViewModels= validateEventEditViewModels;
            _userAuthorizationService = userAuthorizationService;
            _imageSizeValidator= imageSizeValidator;
    }
        public static EventControllerBuilder FullyMockedInstance()
        {
            return new EventControllerBuilder(Mock.Of<IImageService>(), Mock.Of<IMediator>(), Mock.Of<IValidateEventEditViewModels>(),
                                                Mock.Of<IUserAuthorizationService>(), Mock.Of<IImageSizeValidator>());
        }
        public static EventControllerBuilder WithSuppliedInstances(IImageService imageService, IMediator mediator, IValidateEventEditViewModels validateEventEditViewModels)
        {
            return new EventControllerBuilder(imageService, mediator, validateEventEditViewModels, Mock.Of<IUserAuthorizationService>(), Mock.Of<IImageSizeValidator>());
        }

        public static EventControllerBuilder CommonNullTestParams()
        {
            return new EventControllerBuilder(null, Mock.Of<IMediator>(), null,
                Mock.Of<IUserAuthorizationService>(), Mock.Of<IImageSizeValidator>());
        }

        public static EventControllerBuilder AllNullParamsInstance()
        {
            return new EventControllerBuilder(null, null, null, null, null);
        }

        public EventControllerBuilder WithNullImageService()
        {
            _imageService = null;
            return this;
        }

        public EventControllerBuilder WithNullValidateEventEditViewModels()
        {
            _validateEventEditViewModels = null;
            return this;
        }
        public EventControllerBuilder WithMediator(IMediator mediatorObject)
        {
            _mediator = mediatorObject;
            return this;
        }
        public EventControllerBuilder WithToday(Func<DateTime> func)
        {
            _dateTimeTodayDate = func;
            return this;
        }

        public EventController Build()
        {
            var container = new EventController(_imageService, _mediator, _validateEventEditViewModels, _userAuthorizationService, _imageSizeValidator);
            if (_dateTimeTodayDate != null)
            {
                container.DateTimeTodayDate = _dateTimeTodayDate;
            }

            return container;
        }
    }
}
