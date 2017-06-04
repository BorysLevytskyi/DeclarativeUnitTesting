using System;
using Domain;
using Domain.Entities;

namespace Tests.Builders
{
    public static class Given
    {
        public static UserStory UserStory(Action<UserStoryBuilder> setup)
        {
            var builder = new UserStoryBuilder();
            setup(builder);
            return builder.Build();
        }

        public static Sprint Sprint(Action<SprintBuilder> setup)
        {
            var builder = new SprintBuilder();
            setup(builder);
            return builder.Build();
        }


        public static User User(Action<UserBuilder> userBuilder)
        {
            var user = new User();
            userBuilder(new UserBuilder(user));
            return user;
        }
    }
}