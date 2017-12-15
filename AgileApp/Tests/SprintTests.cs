using AgileApp.Tests.Builders;
using FluentAssertions;
using Xunit;

namespace AgileApp.Tests
{
    public class SprintTests
    {
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
                 .UserStory(us => us.Estimated(5).AssignedTo(jack)));

            sprint.CalculateTotalEstimate().Should().Be(10);

            sprint.CalculateTotalEstimateFor(bob.UserId).Should().Be(5);
            
            sprint.CalculateTotalEstimateFor(jack.UserId)
                .Should().Be(5);
        }
    }
}