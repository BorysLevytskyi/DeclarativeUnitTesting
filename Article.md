# Declarative Unit Testing
TODO: Write Preface 
TODO: Write about importance of clean understandable unit tets
TODO: describe example domain - Scrum

## Create Entities using fluent syntax
TODO: Describe the a fluent way of generating entities using domain language
- why not autofixture?
- why it is important to always have consistent object

```C#

// User Story test
[Test]
public void Given_story_in_backlog_when_scheduling_estimated_story_should_move_to_scheduled_state()
{
    // Specify only important parts to the test case. Everything else comes from Default template
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
TODO: write about facade class to all generators
- at any point entity generated is alwats correct thanks to default templates

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
A Builder object provides fluent entity creation interface as a series of chainable methods using *domain language*. This means that builder should not contain a chainable method per entity property but rather method per entity aspect. Builder should not provide interface where object might be created in the inconsistent state. 

Fo example if starting of the `Sprint` means setting up `StartedAt` and `StartedByUserId` properties then respective `SprintBuilder` should provide single 

`Given.Sprint(s => s.Started(date, user))` 

instead of 

`Given.Sprint(s => s.StartedAt(date).StartedByUserId(userId))`. 

It is important that builder uses **domain language** instead of just provide the chainable methods that would simple correspond to each property of the entity. You can add another overload for this chainable method to always set start date to be `DateTime.Now` for test cases where actual date doesn't matter but it is important that it is always set if this sprint is considered to be started - `Given.Sprint(s => s.Started(user)`

Builders define your vocabulary of expressing your entities state. It is also important that builder does internal validation of each state modification of an entity to make sure it is always in the consistent state. This means that part of the business logic has to be implemented in builders. This still pays off very well as project grows.


Example of UserStoryBuilder
```C#
public class UserStoryBuilder
{
    private readonly UserStory _story = new UserStory();

    public UserStoryBuilder NotStarted()
    {
        _story.State = UserStoryState.InBacklog;
        return this;
    }

    public UserStoryBuilder AssignedTo(Assignee assignee)
    {
        _story.Assignee = assignee;
        return this;
    }
  
    public UserStoryBuilder AssignedTo(string name, string email = null)
    {
        return AssignedTo(new Assignee(Identity.Next(), name, email ?? $"{name}@domain.com"));
    }

    public UserStoryBuilder Assigned()
    {
        return AssignedTo("Bob");
    }

    public UserStoryBuilder Unassigned()
    {
        _story.Assignee = null;
        return this;
    }

    public UserStoryBuilder Estimated(StoryPoint storyPoints)
    {
        _story.Estimate = storyPoints;
        return this;
    }

    public UserStoryBuilder Estimated()
    {
        return Estimated(2.StoryPoints());
    }

    public UserStoryBuilder Unestimated()
    {
        _story.Estimate = null;
        return this;
    }

    public UserStoryBuilder InBacklog()
    {
        _story.State = UserStoryState.InBacklog;
        return this;
    }
    
    public UserStory Build()
    {
        return _story;
    }
}
```
### Default Templates
Ideally every unit test should setup those aspects of entity which are direcly tied to it's test case. For example Test Case might not care what name does current user have, but from the code standpoint user must have a name or null reference exception will be thrown. Setting up all required entity attributes in each test case will add more clutter to the test *and more importantly* it will prevent reader from distinguishing which attributes are important to this particular test case and which attributes are set just to make code work. 

Correct generic entity must be created by `Given` generator even in case when no attributes are specified trough fluent interface. It is important that this generic entity is always the same and predictable. Developers must understand which is default state of generic entity before they start to specify important attributes. Fo example every user story that is created is always unassigned and unestimated and has some generic title.

Since the builder itself only provides domain language to express state of the entities, there has to be someone that will use it to create generic entities. This is `DefaultTemplate`'s job. `Given` class, which is a facade for all builders, applies default template for each entity constructor method before executing it. What is good about using Actions and fluent interface is that actions can be combined with using `+` operator in C# that creates [MulticastDelegate](https://msdn.microsoft.com/en-us/library/system.multicastdelegate(v=vs.110).aspx) action as a result. Invokation of this new `Action` delegate will invoke those two method one after another. 

So instead of calling `build(builder)` we can call `(DefaultTemplate.User + build)(builder)`. I added `StartWith` extension method that essentially does the same.

```C#
public static User User(Action<UserBuilder> build = null)
{
    var userBuilder = new UserBuilder();
    build.StartWith(DefaultTemplate.User)(builder);
    return user;
}
```
I usually have one class called `DefaultTemplate` with static methods for every entity that use builder to declare default template for each entity. Thus every developer know if he calls `Given.User()` he will receive user that is declared in a default template class. 

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

## Mocking Dependencies: Specs and Fixtures to Setup Mocks
TODO: write about specs
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
TODO: write about fixtures
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
TODO: write about specs
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
## More adavanced scenarios
TODO: Auto-setup specs
TODO: Custom default templates per fixture
