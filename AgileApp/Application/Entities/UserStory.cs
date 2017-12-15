using System.Collections.Generic;
using System.Linq;
using AgileApp.Application.Exceptions;

namespace AgileApp.Application.Entities
{
    public class UserStory
    {
        public UserStory (int id, string title)
        {
            Id = id;
            Title = title;
            State = UserStoryState.InBacklog;
        }

        public UserStory()
        {
        }

        public int Id { get; set; }

        public int? SprintId { get; set; }

        public int? Estimate { get; set; }

        public string Title { get; set; }

        public Assignee Assignee { get; set; }

        public UserStoryState State { get; set; }

        public void Assign (Assignee assignee)
        {
            Assignee = assignee;
        }

        public void Schedule (int sprintId)
        {
            if (!Estimate.HasValue)
            {
                throw new UserStoryNotEstimatedException();
            }

            TransitionTo(UserStoryState.Scheduled);
            SprintId = sprintId;
        }

        public void Start()
        {
            TransitionTo(UserStoryState.InProgress);
        }

        public void Complete()
        {
            TransitionTo(UserStoryState.Done);
        }

        public void Accept()
        {
            TransitionTo(UserStoryState.Accepted);
        }

        public void Reject()
        {
            TransitionTo(UserStoryState.Rejected);
        }

        public string ToShortString()
        {
            return $"{Title} @{Assignee.Name}";
        }

        private void TransitionTo(UserStoryState nextState)
        {
            if (!GetAllowedStateTransitions (State).Contains (nextState))
            {
                throw new InvalidUserStoryStateException(State, nextState);
            }

            State = nextState;
        }

        private static IEnumerable<UserStoryState> GetAllowedStateTransitions(UserStoryState currentState)
        {
            switch (currentState)
            {
                case UserStoryState.InBacklog:
                    yield return UserStoryState.Scheduled;
                    break;
                case UserStoryState.Scheduled:
                    yield return UserStoryState.InProgress;
                    yield return UserStoryState.InBacklog;
                    break;
                case UserStoryState.InProgress:
                    yield return UserStoryState.InBacklog;
                    yield return UserStoryState.Scheduled;
                    yield return UserStoryState.Done;
                    break;
                case UserStoryState.Done:
                    yield return UserStoryState.InProgress;
                    yield return UserStoryState.Accepted;
                    yield return UserStoryState.Rejected;
                    break;
                case UserStoryState.Accepted:
                    yield break;
                case UserStoryState.Rejected:
                    yield break;
            }
        }
    }
}