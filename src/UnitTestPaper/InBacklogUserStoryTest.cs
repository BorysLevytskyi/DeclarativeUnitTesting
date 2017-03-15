using Domain.Entities;
using FluentAssertions;
using NUnit.Framework;
using Tests.Builders;

namespace Tests
{
    [TestFixture]
    public class InBacklogUserStoryTests
    {
        [Test]
        public void When_scheduling_estimated_story_should_move_to_scheduled_state()
        {
            var userStory = Given.UserStory(s => s.InBacklog().Estimated());

            userStory.Schedule();

            userStory.State.Should().Be(UserStoryState.Scheduled);
        }
    }
}