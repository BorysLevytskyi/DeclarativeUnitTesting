using AgileApp.Domain.Entities;

namespace AgileApp.Tests.Builders
{
    public class UserStoryBuilder
    {
        private readonly UserStory _story = new UserStory();

        public UserStoryBuilder NotStarted ()
        {
            _story.State = UserStoryState.InBacklog;
            return this;
        }

        public UserStoryBuilder AssignedTo (Assignee assignee)
        {
            _story.Assignee = assignee;
            return this;
        }

        public UserStoryBuilder AssignedTo (string name, string email = null)
        {
            return AssignedTo (new Assignee (Identity.Next (), name, email ?? $"{name}@domain.com"));
        }

        public UserStoryBuilder Unassigned ()
        {
            _story.Assignee = null;
            return this;
        }

        public UserStory Build ()
        {
            return _story;
        }

        public UserStoryBuilder Estimated (int storyPoints)
        {
            _story.Estimate = new StoryPoint(storyPoints);
            return this;
        }

        public UserStoryBuilder Estimated()
        {
            return Estimated(2);
        }

        public UserStoryBuilder Unestimated ()
        {
            _story.Estimate = null;
            return this;
        }

        public UserStoryBuilder Assigned()
        {
            return AssignedTo("Bob");
        }

        public UserStoryBuilder InBacklog ()
        {
            _story.State = UserStoryState.InBacklog;
            return this;
        }

        public UserStoryBuilder Id(int id)
        {
            _story.Id = id;
            return this;
        }

        public UserStoryBuilder Title(string title)
        {
            _story.Title = title;
            return this;
        }
    }
}