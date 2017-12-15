using AgileApp.Domain.ApplicationServices;
using AgileApp.Domain.Repositories;
using Moq;

namespace AgileApp.Tests
{
    public class SprintServiceSpec
    {
        public SprintServiceFixture Fixture { get; } = new SprintServiceFixture ();
        private Mock<IUserRepository> UserRepoMock { get; } = new Mock<IUserRepository> ();
        private Mock<ISprintRepository> SprintRepoMock { get; } = new Mock<ISprintRepository> ();
        private Mock<IEventPublisher> EventPublisherMock { get; } = new Mock<IEventPublisher> ();
        
        public SprintServiceFixture.Builder Given ()
        {
            return new SprintServiceFixture.Builder (Fixture);
        }
        
        private void SetupFixture ()
        {
            UserRepoMock.Setup (mx => mx.GetById(Fixture.User.Id))
                .Returns (Fixture.User);
            
            SprintRepoMock.Setup (mx => mx.GetById(Fixture.Sprint.Id))
                .Returns(Fixture.Sprint);
            
            EventPublisherMock.Setup (mx =>
                    mx.Publish(It.IsAny<object>())).Callback<object>(Fixture.PublishedEvents.Add);
        }
        
        protected SprintService CreateSut ()
        {
            SetupFixture();
            
            return new SprintService(
                SprintRepoMock.Object,
                UserRepoMock.Object,
                EventPublisherMock.Object);
        }
    }
}