using System;
using CompanyName.ProjectName.Core.Abstractions.Repositories;
using CompanyName.ProjectName.Core.Abstractions.Services;

namespace CompanyName.ProjectName.Infrastructure.Services
{
    public class MessagesService : IMessagesService
    {
        public MessagesService(IMessagesRepository messagesRepository)
        {
            MessagesRepository = messagesRepository ?? throw new ArgumentNullException(nameof(messagesRepository));
        }

        public IMessagesRepository MessagesRepository { get; }

        // Add business logic here
    }
}
