﻿using MediatR;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class OrganizationIdByTaskIdQueryAsync : IAsyncRequest<int>
    {
        public int TaskId { get; set; }
    }
}