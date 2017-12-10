using System;

namespace AgileApp.Tests.Builders
{
    public static class Given
    {
        public static UserStory UserStory (Action<UserStoryBuilder> build = null)
        {
            var builder = new UserStoryBuilder ();
            build.StartWith (DefaultTemplate.UserStory) (builder);
            return builder.Build ();
        }

        public static Sprint Sprint (Action<SprintBuilder> build = null)
        {
            var builder = new SprintBuilder ();
            build.StartWith (DefaultTemplate.Sprint) (builder);
            return builder.Build ();
        }

        public static User User (Action<UserBuilder> build = null)
        {
            var userBuilder = new UserBuilder ();
            build.StartWith (DefaultTemplate.User) (build);
            return user;
        }
    }
    public class DefaultTemplate
    {
        public static void User (UserBuilder user)
        {
            user.Named ("John Smith").Email ("jon.smith@corp.com");
        }

        public static void UserStory (UserStoryBuilder userStory)
        {
            userStory.Unassigned ().Unestimated ();
        }
    }

    public static class ActionExtensions
    {
        public static Action<T> StartWith<T> (this Action<T> action, Action<T> preceedingAction)
        {
            return preceedingAction + action.EmptyIfNull ();
        }

        public static Action<T> EmptyIfNull<T> (this Action<T> action)
        {
            return action ?? Empty<T>;
        }

        public static void Empty<T> (T arg)
        {

        }
    }
}