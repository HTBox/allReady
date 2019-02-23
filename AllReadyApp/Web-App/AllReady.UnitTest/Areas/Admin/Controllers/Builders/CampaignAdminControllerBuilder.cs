using System;
using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.ViewModels.Validators;
using AllReady.Services;
using MediatR;
using Moq;

namespace AllReady.UnitTest.Areas.Admin.Controllers.Builders
{
    public class CampaignAdminControllerBuilder
    {
        private Func<DateTime> _dateTimeTodayDate;

        private readonly IMediator _mediator;
        private readonly IImageService _imageService;
        private IImageSizeValidator _imageSizeValidator;

        private CampaignAdminControllerBuilder(IMediator mediator, IImageService imageService, IImageSizeValidator imageSizeValidator)
        {
            _mediator = mediator;
            _imageService = imageService;
            _imageSizeValidator = imageSizeValidator;
        }

        public static CampaignAdminControllerBuilder AllNullParamsInstance()
        {
            return new CampaignAdminControllerBuilder(null, null, null);
        }

        public static CampaignAdminControllerBuilder WithMediator(IMediator mediatorObject)
        {
            return new CampaignAdminControllerBuilder(mediatorObject, Mock.Of<IImageService>(), null);
        }

        public static CampaignAdminControllerBuilder WithInstances(IMediator mediatorObject, IImageService imageService)
        {
            return new CampaignAdminControllerBuilder(mediatorObject, imageService, null);
        }

        public CampaignAdminControllerBuilder WithImageSizeValidator(IImageSizeValidator imageSizeValidator)
        {
            _imageSizeValidator = imageSizeValidator;
            return this;
        }

        public CampaignAdminControllerBuilder WithToday(Func<DateTime> func)
        {
            _dateTimeTodayDate = func;
            return this;
        }

        public CampaignController Build()
        {
            var controller = new CampaignController(_mediator, _imageService, _imageSizeValidator);

            if (_dateTimeTodayDate != null)
            {
                controller.DateTimeNow = _dateTimeTodayDate;
            }

            return controller;
        }
    }
}
