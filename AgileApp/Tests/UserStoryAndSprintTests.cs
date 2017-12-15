using AgileApp.Domain.Entities;
using AgileApp.Tests.Builders;
using FluentAssertions;
using Xunit;

namespace AgileApp.Tests
{
    public class UserStoryAndSprintTests
    {
        // User Story test
        [Fact]
        public void Given_story_in_backlog_when_scheduling_estimated_story_should_move_to_scheduled_state ()
        {
            // Specify only important parts to the test case. 
            // Everything else comes from Default template
            var userStory = Given.UserStory (s => s.InBacklog ().Estimated ());

            int sprintId = 1;

            userStory.Schedule (sprintId);

            userStory.SprintId.Should ().Be (sprintId);
            userStory.State.Should ().Be (UserStoryState.Scheduled);
        }

        // Sprint Test
        [Fact]
        public void Should_get_total_estimate_and_per_assignee ()
        {
            var bob = Given.Assignee(a => a.Named("Bob"));
            var jack = Given.Assignee(a => a.Named("Jack"));

            var sprint = Given.Sprint (s =>
                s.UserStory(us => us.Estimated(2).AssignedTo(bob))
                 .UserStory(us => us.Estimated(3).AssignedTo(bob))
                 .UserStory(us => us.Unestimated().AssignedTo(jack))
                 .UserStory(us => us.Estimated(5).AssignedTo (jack)));

            sprint.CalculateTotalEstimate().Should().Be(10.StoryPoints());

            sprint.CalculateTotalEstimateFor(bob.UserId).Should().Be(5.StoryPoints());
            
            sprint.CalculateTotalEstimateFor(jack.UserId)
                .Should().Be(5.StoryPoints());
        }
    }
}