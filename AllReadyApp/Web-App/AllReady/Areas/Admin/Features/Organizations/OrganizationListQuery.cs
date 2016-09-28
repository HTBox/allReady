﻿using System.Collections.Generic;
using AllReady.Areas.Admin.ViewModels.Organization;
using MediatR;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationListQuery : IAsyncRequest<List<OrganizationSummaryViewModel>>
    {
    }
}