using System;
using AgileApp.Application.Entities;
using AgileApp.Application.Exceptions;
using AgileApp.Tests.Builders;
using FluentAssertions;
using Xunit;

namespace AgileApp.Tests
{
    public class UserStoryTests
    {
        [Fact]
        public void Given_story_in_backlog_when_scheduling_estimated_story_should_move_to_scheduled_state ()
        {
            // Specify only important parts to the test case. 
            // Everything else comes from Default template
            var userStory = Given.UserStory (s => s.InBacklog().Estimated());

            int sprintId = 1;

            userStory.Schedule (sprintId);

            userStory.SprintId.Should().Be(sprintId);
            userStory.State.Should().Be(UserStoryState.Scheduled);
        }

        [Fact]
        public void Should_throw_on_illegal_state_transition()
        {
            var story = Given.UserStory(s => s.InState(UserStoryState.Accepted));
            
            Action actSchedule = () => story.Start();
            actSchedule.ShouldThrow<InvalidUserStoryStateException>();
        }
    }
}