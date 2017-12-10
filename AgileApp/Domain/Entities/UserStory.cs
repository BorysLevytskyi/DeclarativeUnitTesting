using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Domain.Exceptions;

namespace Domain.Entities
{
    public class UserStory
    {
        private readonly UserStoryData _data;

        public UserStory(int id, string title)
        {
            _data = new UserStoryData
            {
                Id = id,
                Title = title,
                State = UserStoryState.InBacklog
            };
        }

        private UserStory(UserStoryData data)
        {
            _data = data;
        }

        public int Id => _data.Id;

        public int? SprintId => _data.SprintId;

        public StoryPoint? Estimate => _data.Estimate;

        public string Title => _data.Title;

        public Assignee Assignee => _data.Assignee;

        public UserStoryState State => _data.State;

        public static UserStory Reconstitute(UserStoryData data)
        {
            return new UserStory(data);
        }

        public void SetEstimate(StoryPoint estimate)
        {
            _data.Estimate = estimate;
        }

        public void Assign(Assignee assignee)
        {
            _data.Assignee = assignee;
        }

        public void Schedule(int sprintId)
        {
            if (!Estimate.HasValue)
            {
                throw new UserStoryNotEstimatedException();
            }

            TransitionTo(UserStoryState.Scheduled);
            _data.SprintId = sprintId;
        }

        public void Start()
        {}

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

        public UserStoryData GetData()
        {
            return _data;
        }

        private void TransitionTo(UserStoryState nextState)
        {
            if (!GetAllowedStateTransitions(State).Contains(nextState))
            {
                throw new InvalidUserStoryStateException(State, nextState);
            }

            _data.State = nextState;
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

    public class UserStoryData
    {
        public Assignee Assignee { get; set; }

        public UserStoryState State { get; set; }

        public StoryPoint? Estimate { get; set; }

        public string Title { get; set; }

        public int Id { get; set; }

        public int? SprintId { get; set; }
    }
}