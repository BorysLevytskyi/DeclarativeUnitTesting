namespace AgileApp.Application.Repositories
{
    public interface IEventPublisher
    {
        void Publish(object anEvent);
    }
}