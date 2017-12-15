using System;
using AgileApp.Application.Entities;
using AgileApp.Framework;

namespace AgileApp.Tests.Builders
{
    public static class Given
    {
        public static UserStory UserStory(Action<UserStoryBuilder> build = null)
        {
            var builder = new UserStoryBuilder ();
            build.StartWith(DefaultTemplate.UserStory)(builder);
            return builder.Build();
        }

        public static Sprint Sprint (Action<SprintBuilder> build = null)
        {
            var builder = new SprintBuilder ();
            build.StartWith(DefaultTemplate.Sprint)(builder);
            return builder.Build();
        }

        public static User User(Action<UserBuilder> build = null)
        {
            var builder = new UserBuilder();
            build.StartWith(DefaultTemplate.User)(builder);
            return builder.Build();
        }

        public static Assignee Assignee(Action<AssigneeBuilder> build)
        {
            var builder = new AssigneeBuilder();
            build.StartWith(DefaultTemplate.Assignee)(builder);
            return builder.Build();
        }
    }
}