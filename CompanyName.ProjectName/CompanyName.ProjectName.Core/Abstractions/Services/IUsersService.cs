using CompanyName.ProjectName.Core.Abstractions.Repositories;

namespace CompanyName.ProjectName.Core.Abstractions.Services
{
    public interface IUsersService
    {
        IUsersRepository UsersRepository { get; }
    }
}
