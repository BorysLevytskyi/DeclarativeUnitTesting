using AgileApp.Domain.Entities;

namespace AgileApp.Domain.Repositories
{
    public interface IUserRepository
    {
        User GetById(int userId);
    }
}