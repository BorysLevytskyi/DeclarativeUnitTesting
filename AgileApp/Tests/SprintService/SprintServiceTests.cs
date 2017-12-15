using AgileApp.Application.Events;
using FluentAssertions;
using Xunit;

namespace AgileApp.Tests
{
    public class SprintServiceTest : SprintServiceSpec
    {
        [Fact]
        public void Should_publish_event_when_starting_sprint ()
        {
            Given()
                .ExistingSprint()
                .User(u => u.Named("John Smith"));

            var sut = CreateSut();
            sut.StartSprint(Fixture.Sprint.Id, Fixture.User.Id);

            Fixture.PublishedEvents.Should()
                .ContainSingle()
                .Which.ShouldBeEquivalentTo (new SprintStartedEvent
                {
                    SprintId = Fixture.Sprint.Id,
                    StartedByUserId = Fixture.User.Id,
                    StartedByUserName = "John Smith"
                });
        }
    }
}