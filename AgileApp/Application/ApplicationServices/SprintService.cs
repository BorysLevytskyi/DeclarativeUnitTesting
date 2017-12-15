using AgileApp.Application.Events;
using AgileApp.Application.Repositories;

namespace AgileApp.Application.ApplicationServices
{
    public class SprintService
    {
        private readonly ISprintRepository _sprintRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEventPublisher _eventPublisher;

        public SprintService(ISprintRepository sprintRepository, IUserRepository userRepository, IEventPublisher eventPublisher)
        {
            _sprintRepository = sprintRepository;
            _userRepository = userRepository;
            _eventPublisher = eventPublisher;
        }

        public void StartSprint(int sprintId, int userId)
        {
            var sprint = _sprintRepository.GetById(sprintId);
            var user = _userRepository.GetById(userId);
            
            sprint.Start();
            _sprintRepository.Persist(sprint);

            _eventPublisher.Publish(new SprintStartedEvent
            {
                SprintId = sprintId,
                StartedByUserId = userId,
                StartedByUserName = user.Name
            });
        }
    }
}