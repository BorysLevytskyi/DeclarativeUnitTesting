using System;
using Domain.Entities;
using Domain.Exceptions;
using FluentAssertions;
using NUnit.Framework;
using Tests.Builders;

namespace Tests
{
    [TestFixture]
    public class UserStoryTests
    {
        [Test]
        public static void Should_create_new_story()
        {
            string title = "Some title";
            int id = 10;
            var story = new UserStory(id, title);
            story.Id.Should().Be(id);
            story.State.Should().Be(UserStoryState.InBacklog);
            story.Title.Should().Be(title);
            story.SprintId.Should().Be(null);
        }

        [Test]
        public void Given_story_in_backlog_when_scheduling_estimated_story_should_move_to_scheduled_state()
        {
            var userStory = Given.UserStory(s => s.InBacklog().Estimated());

            int sprintId = 1;

            userStory.Schedule(sprintId);

            userStory.SprintId.Should().Be(sprintId);
            userStory.State.Should().Be(UserStoryState.Scheduled);
        }

        [Test]
        public void Given_story_in_backlog_when_scheduling_unestimated_story_should_throw()
        {
            var userStory = Given.UserStory(s => s.InBacklog().Unestimated());

            Action actSchedule = () => userStory.Schedule(1);
            actSchedule.ShouldThrow<UserStoryNotEstimatedException>()
                .And.Message.Should().Be("User story is not estimated");
        }
    }
}