namespace AgileApp.Domain.Repositories
{
    public interface IEventPublisher
    {
        void Publish(object anEvent);
    }
}