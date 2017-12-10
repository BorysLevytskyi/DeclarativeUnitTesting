using Domain;
using Domain.Entities;

namespace Tests.Builders
{
    public class UserStoryBuilder
    {
        private readonly UserStoryData _storyData = new UserStoryData ();

        public UserStoryBuilder NotStarted ()
        {
            _storyData.State = UserStoryState.InBacklog;
            return this;
        }

        public UserStoryBuilder AssignedTo (Assignee assignee)
        {
            _storyData.Assignee = assignee;
            return this;
        }

        public UserStoryBuilder AssignedTo (string name, string email = null)
        {
            return AssignedTo (new Assignee (Identity.Next (), name, email ?? $"{name}@domain.com"));
        }

        public UserStoryBuilder Unassigned ()
        {
            _storyData.Assignee = null;
            return this;
        }

        public UserStory Build ()
        {
            return UserStory.Reconstitute (_storyData);
        }

        public UserStoryBuilder Estimated (StoryPoint storyPoints)
        {
            _storyData.Estimate = storyPoints;
            return this;
        }

        public UserStoryBuilder Estimated ()
        {
            return Estimated (2. StoryPoints ());
        }

        public UserStoryBuilder Unestimated ()
        {
            _storyData.Estimate = null;
            return this;
        }

        public UserStoryBuilder Assigned ()
        {
            return AssignedTo ("Bob");
        }

        public UserStoryBuilder InBacklog ()
        {
            _storyData.State = UserStoryState.InBacklog;
            return this;
        }
    }
}