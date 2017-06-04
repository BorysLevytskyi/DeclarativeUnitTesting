using Domain;
using Domain.Entities;
using FluentAssertions;
using NUnit.Framework;
using Tests.Builders;
using Moq;
using System;
using System.Collections.Generic;

namespace Tests
{
    [TestFixture]
    public class SprintTests
    {
        [Test]
        public void Should_get_total_estimate_and_per_assignee()
        {
            var bob = Given.Assignee(a => a.Named("Bob"));
            var jack = Given.Assignee(a => a.Named("Jack"));

            var sprint = Given.Sprint(s =>
                s.UserStory(us => us.Estimated(2.StoryPoints()).AssignedTo(bob))
                 .UserStory(us => us.Estimated(3.StoryPoints()).AssignedTo(bob))
                 .UserStory(us => us.Unestimated().AssignedTo(jack))
                 .UserStory(us => us.Estimated(5.StoryPoints()).AssignedTo(jack)));

            sprint.TotalEstimate.Should().Be(10.StoryPoints());

            sprint.GetTotalEstimateForStoriesAssignedTo(bob).Should().Be(5.StoryPoints());
            sprint.GetTotalEstimateForStoreisAssignedTo(jack).Should().Be(5.StoryPoints());
        }
    }

    [TestFixture]
    public class SprintServiceTest : SprintServiceSpec
    {
        [Test]
        public void Should_start_srint_and_publish_event() 
        {
            Given()
                .User(u => u.Named("John Smith"))
                .ExistingSprint();

            var sut = CreateSut();
            sut.StartSprint(Fixture.Sprint.Id, Fixture.User.Id);

            Fixture.PublishedEvents.Should().ContainSingle()
                   .Which.ShouldBeEquivalentTo(new SprintStartedEvent
                   {
                       SprintId = Fixture.Sprint.Id,
                       StartedByUserId = Fixture.User.Id,
                       StartedByUserName = "John Smith"
                   });
        }
    }

    public class SprintServiceSpec 
    {
        public SprintServiceFixture Fixture { get; } = new SprintServiceFixture();

        private Mock<IUserRepository> UserRepoMock { get; } = new Mock<IUserRepository>();

        private Mock<ISprintRepository> SprintRepoMock { get; } = new Mock<ISprintRepository>();

        private Mock<IEventPublisher> EventPublisherMock { get; } = new Mock<IEventPublisher>();
        
        public SprintServiceFixture.Builder Given() 
        {
            return new SprintServiceFixture.Builder(Fixture);
        }

        private void SetupFixture()
        {
            UserRepoMock.Setup(mx => mx.GetById(Fixture.User.Id)).Returns(Fixture.User);  
            SprintRepoMock.Setup(mx => mx.GetById(Fixture.Sprint.Id)).Returns(Fixture.Sprint);
            EventPublisherMock.Setup(mx => mx.Publish(It.IsAny<object>())).Callback<object>(Fixture.PublishedEvents.Add);
        }

        protected SprintService CreateSut() 
        {
            return new SprintService(SprintRepoMock.Object, UserRepoMock.Object, null, EventPublisherMock.Object);
        }
    }

    public class SprintServiceFixture
    {
        public Sprint Sprint { get; private set; }

        public User User { get; private set; }

        public IList<object> PublishedEvents { get; } = new List<object>();

        public class Builder
        {
            readonly SprintServiceFixture _fixture;

            public Builder(SprintServiceFixture fixture)
            {
                _fixture = fixture;
            }


            public Builder User(Action<UserBuilder> build) 
            {
                _fixture.User = Given.User(build);
                return this;
            }

            public Builder ExistingSprint(Action<SprintBuilder> build = null)
            {
                _fixture.Sprint = Given.Sprint(build);
                return this;
            }
        }
    }
}