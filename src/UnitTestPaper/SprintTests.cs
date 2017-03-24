using Domain.Entities;
using FluentAssertions;
using NUnit.Framework;
using Tests.Builders;

namespace Tests
{
    [TestFixture]
    public class SprintTests
    {
        [Test]
        public void Should_get_total_estimate()
        {
            var sprint = Given.Sprint(s =>
                s.UserStory(us => us.Estimated(2.StoryPoints()))
                 .UserStory(us => us.Estimated(3.StoryPoints()))
                 .UserStory(us => us.Unestimated())
                 .UserStory(us => us.Estimated(5.StoryPoints())));

            sprint.TotalEstimate.Should().Be(10.StoryPoints());
        }
    }
}