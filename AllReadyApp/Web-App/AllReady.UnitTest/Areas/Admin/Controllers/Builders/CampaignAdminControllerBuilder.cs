using System;
using AllReady.Areas.Admin.Controllers;
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

        private CampaignAdminControllerBuilder(IMediator mediator, IImageService imageService)
        {
            _mediator = mediator;
            _imageService = imageService;
        }

        public static CampaignAdminControllerBuilder AllNullParamsInstance()
        {
            return new CampaignAdminControllerBuilder(null, null);
        }

        public static CampaignAdminControllerBuilder WithMediator(IMediator mediatorObject)
        {
            return new CampaignAdminControllerBuilder(mediatorObject, null);
        }

        public static CampaignAdminControllerBuilder WithInstances(IMediator mediatorObject, IImageService imageService)
        {
            return new CampaignAdminControllerBuilder(mediatorObject, imageService);
        }

        public CampaignAdminControllerBuilder WithToday(Func<DateTime> func)
        {
            _dateTimeTodayDate = func;
            return this;
        }

        public CampaignController Build()
        {
            var controller = new CampaignController(_mediator, _imageService);

            if (_dateTimeTodayDate != null)
            {
                controller.DateTimeNow = _dateTimeTodayDate;
            }

            return controller;
        }
    }
}
