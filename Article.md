# Declarative Unit Testing
Unit Tests can provide great value in software development but only if done right. And by right I mean that unit test should be written according to these two principles
1. **Concise** - a test have to be understandable and clearly convey what behavior does it test and what particular aspect is of it in each of the test cases
2. **Stable** - unit test must withstand inner implementation refactorings of the component. If change in **how** component does it's job (without change of the behavior itself) requires unit test to change than there is nothing that will say that component is not broken and still does it's jub good.

Over the past 10 years working as a software developer I spent 8 or so years working with unit tests and I've seen those which help development and provide a great value and those which made it even harder. In this article I'd like to share with you my current approach with the hope it will be helpful to you as it was for me.

### How to Make Tests Concise

Usually unit tests suffer from having a lot of boilerplate code that is there mainly to create test entity object, get them in desired state and then connect them to mock or fake objects. Another downside of having such piping code right there is the test is it makes it hard to see which attributes of the test entity is actually important for this particular test case and which attributes are set to make the code work. Far more often than not in my work and saw unit tests where it was really hard to figure out whic behavior those tests were testing which made maintainining of them a nightmare. Usually when such tests are failing developers try to do as little as possible to make them green again adding hacks and short cuts making those tests even worth until someone just gives up and ignores them.

**Test cases must be as pure as possible** containing only code that expresses test case itself and it's most important aspects. Like which attributes of the entity is really important here. For example if I test `Retire` method on the user which can only be done upon active users this means that my test cases should say that user is in `Active` state, leaving all properties like `Name`, `Address` and `DateOfBirth` and other attributes that constitute a valid user to be specified somewhere else as it is not important which name, or address does user have in this particular test case. But it is really important for this test case that user is an active user. This means that this boilerplate code has to go somewhere else where it can be reused and reduced to a minimum. 

Having to write boilerplate code in each test *significantly reduces coverage* because developers just don't want to write a lot of unit tests in this way doing repetitive work over and over.

A while back I thought of having a way to generate generic entities, for example `User`, by having very simple facade class. Like `var user = Create.User()` method which will create generic `User` entity with all fields set up to arbitrary (within correct boundaries) values. Then I'd "override" any attributes I deem important for every particular test case. Something like:
```
var genericUser = Create.User();
var retiredBob = Create.User(u => u.Named("Bob").Retired(2.DaysAgo()))
```
I came with this facade class with factory methods for each domain entity that accept `Action<EntityBuilder>`. Every Entity has it's own builder to provide a **domain** language of expressing it's attributes and state.

In this article I'm gonna use Agile Scrum domain having Sprints, UserStories, Users as an example. 

### Stability

Generating always correct entities by your builders make tests more stable. I've seen situations where developers set only properties of an entity that were used in particular piece of code leaving all other properties to be null, since they weren't read or set in the sut component. Later when that class was changed to work with additional properties on that entity - all tests began to fail due to `NullReferenceException` being thrown because that property wasn't set up in tests. Developer then had do go back to all those tests and set that property in each unit test. 

Having boilerplate code removed from the test cases themselves also adds stability to unit tests. Boilerplate code is put in one place and shared between tests and is usually easy to change without affecting the test cases themselves. When tests are written in declarative behavior-driven style changes in infrastructural code are much less likely to affect them. [Gherkin](https://github.com/cucumber/cucumber/wiki/Gherkin) language is a good example. It is the most ultimate case of pushing all the boilerplate code out. Do the degree that Gherkin tests can be run in any platform that supports it like .NET, Java or Javascript. What I'm using in my unit tests looks somewhat like `Gherkin` but is aimed at writing  unit tests, while `Gherkin` is used in writing integration tests. No doubt it was inspiration for my approach to unit testing. 

### Why not use AutoFixture
AutoFixture is a good tool for generating entities "hydrated" with random attributes but the problem is in this randomness. I want my Entities to be created in always correct state. And this correct state might mean that certain group of properties can be setup with values that agree with each other. You can do this with AutoFixture customizations but once your start doing it you now understand that you just using AutoFixture to kickoff generation and doing all the important staff in your customization code. Beside I want to have a fluent interface of chainable methods that are **not** per-property based. I'll tell you why later in this article.

## Create Entities using fluent syntax
I'm a big fan of Given-When-Then approach of structuring unit tests so I've been calling my facade class `Given` instead of `Create` in recent projects. I like my unit tests to be sound the same as just plain english explanation of the test case.

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

    sprint.GetTotalEstimateFor(bob.Id).Should().Be(5.StoryPoints());
    sprint.GetTotalEstimateFor(jack.Id).Should().Be(5.StoryPoints());
}
```

### Fluent Entity Generator
I like [Fluent Assertions](http://fluentassertions.com/) library and how it makes tests much better, cleaner and readable. A while back I thought of a way to use the same fluent syntax for expressing entities involved in the test case. 

The idea was to have single facade class called `Given` that provides a set of method for creating entities from a domain model. Each method accepts `Action<EntityBuilder>` which is used for expressing entity state. This facade use two components to do it's job
- Entity Builders
- Default Templates

Example of `Given` facade class for Agile Scrum domain
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

A Builder object provides fluent entity creation interface as a series of chainable methods using *domain language*. This means that builder should not contain a chainable method per entity property but rather method per entity aspect. It should not provide interface where object might be created in the inconsistent state. 

Fo example if starting of the `Sprint` means setting up `StartedAt` and `StartedByUserId` properties then respective `SprintBuilder` should provide single 

`Given.Sprint(s => s.Started(date, user))` 

instead of 

`Given.Sprint(s => s.StartedAt(date).StartedByUserId(userId))`. 

because you may choose (knowingly or unknowingly) not to set `StartedByUserId` property and now you have object in the inconsistent state. 

It is important that builder uses **domain language** instead of just provide the chainable methods that would simple correspond to each property of the entity. You can add another overload for this chainable method to always set start date to be `DateTime.Now` for test cases where actual date doesn't matter but it is important that it is always set if this sprint is considered to be started - `Given.Sprint(s => s.Started(user)`

Builders define your vocabulary of expressing your entities state. It is also important that builder does internal validation of each state modification of an entity to make sure it is always in the consistent state. Of course to make that happen a part of the business logic has to be implemented in builders. This still pays off very well as project grows. 

Builder's domain-specific vocabulary also server important role of communicating of business rules especially when the code being test is doing bad job of it by being messy and suffering primitive obsession. 

#### Legacy Projects
*Before refactoring production code towards clarity* it is safer and easier first to refactor unit tests to use declarative style to express business rules after that you will have advantage of:
1. Better understanding of what and why production code does
2. Having reliable and maintainable tests suite for it 

so you can start refactoring of actual production code and you'll know that your tet are actually helping you. I've seen like test cases that were 30-50 lines method became 10 liners (including "space" lines). Much easier to understand and change.

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
As I mentioned earlier - ideally every unit test should setup those aspects of entity which are directly tied to it's test case. For example a particular test case might not care what name does current user have, but from the code standpoint user must have a name or `NullReferenceException` will be thrown. Setting up all required entity attributes in each test case will add more clutter to the test *and more importantly* it will prevent reader from distinguishing which attributes are important to this particular test case and which attributes are set just to make code work. 

In my case always correct generic entity is created by `Given` facade class. It is important that this generic entity is always the same and in some agreed upon state. Developers must understand which is `default` state of this entity before they start to override important attributes. For example every user story that is created is always unassigned and unestimated and has some generic title.

Since the builder itself only provides domain language to express state of the entities, there has to be someone that will use it to create default  entities. This is `DefaultTemplate`'s job. `Given` class, which is a facade for all builders, applies default template for each entity constructor method before executing it. What is good about using Actions and fluent interface is that actions can be combined with using `+` operator in C# that creates [MulticastDelegate](https://msdn.microsoft.com/en-us/library/system.multicastdelegate(v=vs.110).aspx) action as a result. Invocation of this new `Action` delegate will invoke those two methods one after another. 

So instead of calling `build(builder)` we can call `(DefaultTemplate.User + build)(builder)`. I added `StartWith` extension method that essentially does the same but looks nicer.

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
        user.Named("John Smith")
            .Email("jon.smith@corp.com")
            .Active()
            .HasARoleOf(RoleType.SysAdmin); // Except some security test cases user always has access to everything by default
    }

    public static void UserStory(UserStoryBuilder userStory)
    {
        var id = Identity.Next();
        userStory.Id(Identity.Next()).Unassigned().Unestimated().Title($"Generic story #{id}")';        
    }
}
```

## Mocking Dependencies: Specs and Fixtures to Setup Mocks
There are cases when just creating entities trough fluent interface is not enough for removing all the boilerplate code from the unit test and that is when you need so setup dependencies and mock indirect inputs and indirect outputs. Setting up mocks right in the test cases adds more clutter and hinders clarity of a unit test. So this boilerplate code has to go somewhere. And this somewhere is what I call a `Spec` base class from which each test class is inherited. It's job is to do:
- Provide you vocabulary for expressing your givens trough `Fixture` object.
- Setup all mocks with entities your created as your givens
- Capture and provide access to indirect outputs crated by your mocks

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
The Fixture is essentially a data model for your tests. For example from looking at `SprintServiceFixture` we can tell that test cases, it is involved in, include a single Sprint and User who does something with it. Fixtures can be different for each test class. 

```C#
public class SprintServiceFixture
{
    public Sprint Sprint { get; private set; }

    public User User { get; private set; }

    public IList<object> PublishedEvents { get; } = new List<object>();

    // Builder code omitted for this example
}
```

There is also `PublishedEvents` property in this fixture which is a place where all indirect outputs of this test go. In current test case indirect outputs are Sprint Events that are published `IEventPublisher` interface. `Spec` class will configure publisher mock to put all events into this collection.

#### Fixture Builders
Fixture also has a `Builder` that provides access to fluent interface for building objects in the fixture. Internally it uses `Given` for building entities. You use it to "hydrate" property objects in the fixture. 
```
public Builder ExistingSprint(Action<SprintBuilder> build = null)
{
    _fixture.Sprint = Given.Sprint(build);
    return this;
}
```
Builder's language is even more specific than `Given`'s language. So in this particular test cases Builder's method for creating a Sprint is called `ExistingSprint` to express this given more accurately.

What's cool about templating trough concatenating `Action` object is that builder can stack it's own fixture-specific templates on top of the Default Template which is again shows how convenient it is to use `Action` based interface to do manipulations with builders.

```
public Builder User(Action<UserBuilder> build) 
{
    _fixture.User = Given.User(build.StartWith(UserTemplate));
    return this;
}    

private void UserTemplate(UserBuilder user)
{
    // By default users have access to the given sprint
    user.HasAccessTo(_fixture.Sprint);
}
```
In example above I don't want to always say that User has access to the given Sprint in every test cases so I just make it part of my Fixture Builder's default template

Example for SprintServiceFixture
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

        public Builder ExistingSprint(Action<SprintBuilder> build = null)
        {
            _fixture.Sprint = Given.Sprint(build);
            return this;
        }

        public Builder User(Action<UserBuilder> build) 
        {
            _fixture.User = Given.User(build.StartWith(UserTemplate));
            return this;
        }    

        private void UserTemplate(UserBuilder user)
        {
            // By default users have access to the given sprint
            user.HasAccessTo(_fixture.Sprint);
        }
    }
}
```
### Spec Base Class
`Spec` class is a place where it all comes together. Every test case it's own or shared spec file. Spec provides access to it's Fixture Builder trough `Given()` method. It also has `CreateSut()` method that will setup all the indirect input mocks to return entities set up in Fixture and also setups indirect output mocks to route all outputs back to Fixture's properties.

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
        return new SprintService(
                SprintRepoMock.Object, 
                serRepoMock.Object, 
                EventPublisherMock.Object);
    }
}
```
## Summary
TODO: Write
