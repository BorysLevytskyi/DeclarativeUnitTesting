using System.Collections.Generic;
using Domain.Entities;

namespace Domain
{
    public class SprintService
    {
        private readonly ISprintRepository _sprintRepo;

        private readonly IEventPublisher _eventPublisher;

        private readonly IUserRepository _userRepo;
        readonly ISecurityService _securityService;

        public SprintService(ISprintRepository sprintRepo, IUserRepository userRepo, ISecurityService securityService, IEventPublisher eventPublisher)
        {
            _securityService = securityService;
            _userRepo = userRepo;
            _eventPublisher = eventPublisher;
            _sprintRepo = sprintRepo;
        }
        
        public void StartSprint(int sprintId, int startedByUserId)
        {
            _securityService.AuthoriseAction(SprintActions.Create, startedByUserId);

            var sprint = _sprintRepo.GetById(sprintId);
            var user = _userRepo.GetById(startedByUserId);

            sprint.Start();

            _eventPublisher.Publish(new SprintStartedEvent { 
                SprintId = sprintId, 
                StartedByUserId = user.Id, 
                StartedByUserName = user.Name 
            });
        }
    }

    public interface ISprintRepository
    {
        Sprint GetById(int id);
    }

    public interface IEventPublisher
    {
        void Publish(object anEvent);   
    }

    public interface IUserRepository 
    {
        User GetById(int id);   
    }

    public interface ISecurityService 
    {
        void AuthoriseAction(SprintActions action, int startedByUserId);
    }

    public enum SprintActions 
    {
        Create,
        Start,
        Close,
        Delete
    }
}