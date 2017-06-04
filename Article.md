# Declarative Unit Testing

## Create Entities using fluent syntax
```C#

// User Story test
[Test]
public void Given_story_in_backlog_when_scheduling_estimated_story_should_move_to_scheduled_state()
{
    var userStory = Given.UserStory(s => s.InBacklog().Estimated());

    int sprintId = 1;

    userStory.Schedule(sprintId);

    userStory.SprintId.Should().Be(sprintId);
    userStory.State.Should().Be(UserStoryState.Scheduled);
}

// Sprint Test
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
```

### Fluent Entity Generator
```C#
public static class Given
{
    public static UserStory UserStory(Action<UserStoryBuilder> build = null)
    {
        var builder = new UserStoryBuilder();
        build.StartWith(DefaultTemplate.UserStory)(builder);
        return builder.Build();
    }

    public static Sprint Sprint(Action<SprintBuilder> build = null)
    {
        var builder = new SprintBuilder();
        build.StartWith(DefaultTemplate.Sprint)(builder);
        return builder.Build();
    }


    public static User User(Action<UserBuilder> build = null)
    {
        var userBuilder = new UserBuilder();
        build.StartWith(DefaultTemplate.User)(build);
        return user;
    }
}
```
### Builders
```C#
public class UserStoryBuilder
{
    private readonly UserStoryData _storyData = new UserStoryData();

    public UserStoryBuilder NotStarted()
    {
        _storyData.State = UserStoryState.InBacklog;
        return this;
    }

    public UserStoryBuilder AssignedTo(Assignee assignee)
    {
        _storyData.Assignee = assignee;
        return this;
    }

    public UserStoryBuilder AssignedTo(string name, string email = null)
    {
        return AssignedTo(new Assignee(Identity.Next(), name, email ?? $"{name}@domain.com"));
    }

    public UserStoryBuilder Unassigned()
    {
        _storyData.Assignee = null;
        return this;
    }

    public UserStory Build()
    {
        return UserStory.Reconstitute(_storyData);
    }

    public UserStoryBuilder Estimated(StoryPoint storyPoints)
    {
        _storyData.Estimate = storyPoints;
        return this;
    }

    public UserStoryBuilder Estimated()
    {
        return Estimated(2.StoryPoints());
    }

    public UserStoryBuilder Unestimated()
    {
        _storyData.Estimate = null;
        return this;
    }

    public UserStoryBuilder Assigned()
    {
        return AssignedTo("Bob");
    }

    public UserStoryBuilder InBacklog()
    {
        _storyData.State = UserStoryState.InBacklog;
        return this;
    }
}
```
### Default temaplates
```C#
public class DefaultTemplate 
{
    public static void User(UserBuilder user) 
    {
        user.Named("John Smith").Email("jon.smith@corp.com");
    }

    public static void UserStory(UserStoryBuilder userStory)
    {
        userStory.Unassigned().Unestimated();        
    }
}
```

## Use Specs and Fixtures to setup mocks
Test method looks nice and clean and is not cluttered with mocks creation

```C#
[TestFixture]
public class SprintServiceTest : SprintServiceSpec
{
    [Test]
    public void Should_publish_event_when_starting_sprint() 
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
```

### Fixtures

```C#
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
```
### Spec files
```C#
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
```
