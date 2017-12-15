using System;
using System.Collections.Generic;
using AgileApp.Application.Entities;
using AgileApp.Framework;
using AgileApp.Tests.Builders;

namespace AgileApp.Tests
{
    public class SprintServiceFixture
    {
        public Sprint Sprint { get; private set; }
        
        public User User { get; private set; }
        
        public List<object> PublishedEvents { get; } = new List<object>();

        public class Builder
        {
            readonly SprintServiceFixture _fixture;
            
            public Builder (SprintServiceFixture fixture)
            {
                _fixture = fixture;
            }
            
            public Builder ExistingSprint (Action<SprintBuilder> build = null)
            {
                _fixture.Sprint = Given.Sprint(build.StartWith(SprintTemplate));
                return this;
            }
            
            public Builder User(Action<UserBuilder> build)
            {
                _fixture.User = Given.User(build.StartWith(UserTemplate));
                return this;
            }

            private void SprintTemplate(SprintBuilder sprint)
            {
                sprint.UserStory("Simlpe Story");
            }
            
            
            private void UserTemplate(UserBuilder user)
            {
                // By default users have access to the given sprint
                user.HasAccessTo (_fixture.Sprint);
            }
        }
    }
}