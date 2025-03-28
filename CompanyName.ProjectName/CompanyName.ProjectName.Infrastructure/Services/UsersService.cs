using System;
using CompanyName.ProjectName.Core.Abstractions.Repositories;
using CompanyName.ProjectName.Core.Abstractions.Services;

namespace CompanyName.ProjectName.Infrastructure.Services
{
    public class UsersService : IUsersService
    {
        public UsersService(IUsersRepository usersRepository)
        {
            UsersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
        }

        public IUsersRepository UsersRepository { get; }

        // Add business logic here
    }
}
