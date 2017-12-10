using System;
using AgileApp.Domain.Entities;

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
}