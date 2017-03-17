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
    }
}