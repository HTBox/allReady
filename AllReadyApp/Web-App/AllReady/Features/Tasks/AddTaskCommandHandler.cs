﻿using System.Threading.Tasks;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.Tasks
{
    public class AddTaskCommandHandler : AsyncRequestHandler<AddTaskCommand>
    {
        private readonly AllReadyContext dataContext;

        public AddTaskCommandHandler(AllReadyContext dataContext)
        {
            this.dataContext = dataContext;
        }

        protected override async Task HandleCore(AddTaskCommand message)
        {
            this.dataContext.Add(message.AllReadyTask);
            await this.dataContext.SaveChangesAsync();
        }
    }
}
