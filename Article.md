# Declarative Unit Testing
Unit Tests can provide great value in software development but only if done right. And by right I mean that unit tests should be written according to these two principles.
1. **Concise** - a unit test has to be understandable and clearly convey what aspect of what behavior is being tested in each case.
2. **Stable** - a unit test must withstand inner implementation refactorings of the component. If a change in **how** the component does it's job (without changing the behavior itself) requires the unit test to change, then there is nothing that will say that component is not broken and still does it's job.

Over the past ten years working as a software developer I spent eight or so years working with unit tests and I've seen those which help development and provide a great value and those which made it even harder. In this article I'd like to share with you my current approach with the hope it will be as helpful to you as it was to me.

### How to Make Tests Concise

Usually unit tests suffer from having a lot of boilerplate code that is there mainly to create test entity objects, get them to the desired state, and then connect them to mock or fake objects. Another downside of having such piping code right there in the test is that it makes it hard to see which attributes of the test entity are actually important for this particular test case and which attributes are set to make the code work. Far more often than not in my work I encountered unit tests where it was really hard to figure out which behavior was being tested. This made maintaining them a nightmare. Usually when such tests fail developers try to do as little as possible to make them green again, adding hacks and short cuts making those tests even worse until eventually the team just gives up and ignores them.

**Test cases must be as pure as possible** containing only code that expresses the test case itself and it's most important aspects. Like which attributes of the entity are really important here. For example if I test the `Retire` method on a user, which can only be done upon active users, this means that my test case should say that the user is in `Active` state, leaving all properties like `Name`, `Address` and `DateOfBirth` and other attributes that constitute a valid user to be specified somewhere else as it is not important which name, or address the user has in this particular test case. But it is really important for this test case that the user is an active user. This means that this boilerplate code has to go somewhere else where it can be reused and reduced to a minimum. 

Having to write boilerplate code in each test *significantly reduces coverage* because developers just don't want to write a lot of unit tests in this way involving repetitive work over and over.

A while back I thought of having a way to generate generic entities, for example `User`, by having a very simple facade class. For example `var user = Create.User()` should create a generic `User` entity with all fields set up to arbitrary values (within correct boundaries). Then I'd "override" those attributes I deem important for every particular test case. Something like:
```
var genericUser = Create.User();
var retiredBob = Create.User(u => u.Named("Bob").Retired(2.DaysAgo()))
```
I came up with this facade class with factory methods for each domain entity that accepts `Action<EntityBuilder>`. Every entity has it's own builder to provide a **domain** language of expressing it's attributes and state.

In this article I'll use the Agile Scrum domain with Sprints, UserStories, and Users as an example. 

### Stability

Generating entities with builders that are always correct makes your tests more stable. I've seen situations where developers set only those properties of an entity which were used in a particular piece of code, leaving all other properties to be null, since they weren't read or set in the sut component. Later when that class was changed to work with additional properties of that entity, all tests began to fail with a `NullReferenceException` being thrown because that property wasn't set up in tests. The team then had do go back to all those tests and set that property in each unit test. 

Having boilerplate code removed from the test cases themselves also adds stability to the unit tests. Boilerplate code is put in one place and shared between tests and is usually easy to change without affecting the test cases themselves. When tests are written in a declarative, behavior-driven style, changes in infrastructure code are much less likely to affect them. The [Gherkin](https://github.com/cucumber/cucumber/wiki/Gherkin) language is a good example. It is the ultimate case of pushing all the boilerplate code out. To the degree that Gherkin tests can be run on any platform that supports it, like .NET, Java or Javascript. What I'm doing in my unit tests looks somewhat like `Gherkin` but is aimed at writing unit tests, while `Gherkin` is used in writing integration tests. No doubt it was an inspiration for my approach to unit testing. 

### Why not use AutoFixture
AutoFixture is a good tool for generating entities "hydrated" with random attributes but the problem is in this randomness. I want my Entities to always be created in a correct state. This correct state might mean that a certain group of properties must be set up with values that agree with each other. You can do this with AutoFixture customizations, but once you start doing it you will understand that you're just using AutoFixture to kickoff generation and doing all the important stuff in your customization code. Besides, I want to have a fluent interface of chainable methods that are **not** per-property based. I'll tell you why later in this article.

## Create Entities using fluent syntax
I'm a big fan of the Given-When-Then approach of structuring unit tests so I've been calling my facade class `Given` instead of `Create` in recent projects. I like my unit tests to sound the same as a plain English explanation of the test case.

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
I like the [Fluent Assertions](http://fluentassertions.com/) library and how it makes tests much better, cleaner and readable. A while back I thought of a way to use the same fluent syntax for expressing entities involved in a test case. 

The idea was to have single facade class called `Given` that provides a set of methods for creating entities from a domain model. Each method accepts an `Action<EntityBuilder>` which is used for expressing entity state. This facade uses two components to do it's job
- Entity Builders
- Default Templates

An example of the `Given` facade class for the Agile Scrum domain
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

A Builder object provides a fluent entity creation interface as a series of chainable methods using the *domain language*. This does not mean that the builder should contain a chainable method per entity property, but rather per entity aspect. It should not provide an interface that would allow the object to be created in an inconsistent state. 

For example, if starting the `Sprint` means assigning `StartedAt` and `StartedByUserId` properties then the `SprintBuilder` should provide a single method 

`Given.Sprint(s => s.Started(date, user))` 

instead of 

`Given.Sprint(s => s.StartedAt(date).StartedByUserId(userId))`. 

This is because you might choose (knowingly or unknowingly) not to set the `StartedByUserId` property and be left with an object in an inconsistent state. 

It is important that the builder use **domain language** instead of just providing the chainable methods that would simply correspond to each property of the entity. You can add another overload for this chainable method to always set start date to be `DateTime.Now` for test cases where the actual date doesn't matter, but it is important that it always gets set if this sprint is considered to be started - `Given.Sprint(s => s.Started(user))`

Builders define your vocabulary for expressing the state of your entities. It is also important for the builder to do internal validation of each state modification of an entity to make sure it is always in a consistent state. Of course to make that happen, a part of the business logic has to be implemented in builders. This still pays off very well as the project grows. 

The builder's domain-specific vocabulary also serves the important role of communicating business rules, especially when the code being test is doing a bad job of it by being messy and suffering primitive obsession. 

#### Legacy Projects
*Before refactoring production code towards clarity* it is safer and easier first to refactor the unit tests to use a declarative style and to express business rules. After that you will have the advantage of:
1. A better understanding of what the production code does and why.
2. A reliable and maintainable suite of tests. 

Now you can start refactoring the actual production code and you'll know that your tests are actually helping you. I've seen test cases that were 30-50 lines reduced to ten liners (including "space" lines). Much easier to understand and change.

UserStoryBuilder Example
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
As I mentioned earlier, ideally every unit test should set up those aspects of an entity which are directly tied to the test case. For example a particular test case might not care what name the current user has, but from the code standpoint a user must have a name or a `NullReferenceException` will be thrown. Setting up all the required entity attributes in each test case adds more clutter to the test, *and more importantly* it prevents the reader from distinguishing which attributes are important to this particular test case and which attributes were set just to make the code work. 

In my tests the correct generic entity is always created by a `Given` facade class. It is important that a generic entity is always the same and in some agreed upon state. Developers must understand what the `default` state of an entity is before they start to override important attributes. For example every user story that is created is always unassigned and unestimated and has some generic title.

Since the builder itself only provides domain language to express the state of the entities, there has to be some way to use it to create default entities. This is `DefaultTemplate`'s job. The `Given` class, which is a facade for all builders, applies the default template for each entity constructor method before executing it. What is good about using Actions and the fluent interface is that actions can be combined with using `+` operator in C#. This results in a [MulticastDelegate](https://msdn.microsoft.com/en-us/library/system.multicastdelegate(v=vs.110).aspx) action. Invocation of this new `Action` delegate will invoke those two methods, one after another. 

So instead of calling `build(builder)`, we can call `(DefaultTemplate.User + build)(builder)`. I added a `StartWith` extension method that essentially does the same but looks nicer.

```C#
public static User User(Action<UserBuilder> build = null)
{
    var userBuilder = new UserBuilder();
    build.StartWith(DefaultTemplate.User)(builder);
    return user;
}
```
I usually have a single `DefaultTemplate` class that groups together static methods for every entity that uses the builder. Thus every developer knows if he calls for example `Given.User()`, he will receive a user that is declared in the default template class. 

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

## Mocking Dependencies: Specs and Fixtures to set up Mocks
There are cases when just creating entities through the fluent interface is not enough for removing all the boilerplate code from the unit test.  That is when you need to set up dependencies and mock indirect inputs and indirect outputs. Setting up mocks right in the test case adds more clutter and hinders the clarity of the unit test. This boilerplate code has to go somewhere. This somewhere is what I call a `Spec` base class, from which each test class is inherited. It's job is to:
- Provide vocabulary for expressing your givens through a `Fixture` object.
- Set up all mocks with entities you created as your givens.
- Capture and provide access to indirect outputs created by your mocks.

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
The Fixture is essentially a data model for your tests. For example, looking at `SprintServiceFixture` we can tell that the test cases it is involved in include a single Sprint and a User who does something with it. Fixtures can be different for each test case. 

```C#
public class SprintServiceFixture
{
    public Sprint Sprint { get; private set; }

    public User User { get; private set; }

    public IList<object> PublishedEvents { get; } = new List<object>();

    // Builder code omitted for this example
}
```

The `PublishedEvents` property in this fixture is where all indirect outputs of this test go. In the example test case indirect outputs are Sprint Events that are published with the `IEventPublisher` interface. The `Spec` class will configure a publisher mock to put all events into this collection.

#### Fixture Builders
The fixture also has a `Builder` that provides access to a fluent interface for building objects. Internally it uses `Given` for building entities. It is used to "hydrate" property objects in the fixture. 
```
public Builder ExistingSprint(Action<SprintBuilder> build = null)
{
    _fixture.Sprint = Given.Sprint(build);
    return this;
}
```
The builder's language is even more specific than `Given`'s language. In this particular test case Builder's method for creating a Sprint is called `ExistingSprint` to express this given more accurately.

What is cool about templating through concatenating `Action` objects is that the builder can stack it's own fixture-specific templates on top of the Default Template.  This again shows how convenient it is to use an `Action` based interface to perform manipulations with builders.

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
In the example above I don't want to have to say that the User has access to the given Sprint every time I write a test case, so I just make it part of my Fixture Builder's default template.

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
The `Spec` class is the place where it all comes together. Every test case has it's own or a shared spec file. The Spec class provides access to it's Fixture Builder through the `Given()` method. It also has a `CreateSut()` method that will set up all the indirect input mocks to return the entities set up in the Fixture.  It also sets up indirect output mocks to route all outputs back to the Fixture's properties.

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
## In Conclusion
Bottom line 
* Push all boilerplate code out of your test cases.
* Use domain language to express preconditions and expectations.
* Use correct and consistent entities in tests the same way they are used in production scenarios.

Code should communicate business rules. The same goes for unit tests. If application code is messy and primitive-obsessed you can begin fixing it with unit tests that use domain language. The tests then can become a foundation for a successful refactoring.
