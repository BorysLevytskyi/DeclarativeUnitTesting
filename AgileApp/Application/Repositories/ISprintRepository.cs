using AgileApp.Application.Entities;

namespace AgileApp.Application.Repositories
{
    public interface ISprintRepository
    {
        Sprint GetById(int sprintId);
        
        void Persist(Sprint sprint);
    }
}