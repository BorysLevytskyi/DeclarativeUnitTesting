using AgileApp.Domain.Entities;

namespace AgileApp.Domain.Repositories
{
    public interface ISprintRepository
    {
        Sprint GetById(int sprintId);
        
        void Persist(Sprint sprint);
    }
}