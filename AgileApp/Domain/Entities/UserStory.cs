using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using AgileApp.Domain.Exceptions;

namespace AgileApp.Domain.Entities
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

        public StoryPoint? Estimate { get; set; }

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
        }

        public void Complete()
        {
        }

        public void Accept()
        {
        }

        public void Reject()
        {
        }

        public string ToShortString()
        {
            return $"{Title} @{Assignee.Name}";
        }

        private void TransitionTo(UserStoryState nextState)
        {
            if (!GetAllowedStateTransitions (State).Contains (nextState))
            {
                throw new InvalidUserStoryStateException (State, nextState);
            }

            State = nextState;
        }

        private static IEnumerable<UserStoryState> GetAllowedStateTransitions (UserStoryState currentState)
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