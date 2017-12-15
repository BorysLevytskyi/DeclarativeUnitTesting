using AgileApp.Application.Entities;

namespace AgileApp.Application.Repositories
{
    public interface IUserRepository
    {
        User GetById(int userId);
    }
}