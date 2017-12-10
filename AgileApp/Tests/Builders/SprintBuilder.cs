using System;
using System.Collections.Generic;
using AgileApp.Domain.Entities;

namespace AgileApp.Tests.Builders
{
    public class SprintBuilder
    {
        private readonly List<UserStory> _userStories = new List<UserStory> ();

        public SprintBuilder UserStory (Action<UserStoryBuilder> buildStory)
        {
            var userStory = Given.UserStory (buildStory);
            _userStories.Add (userStory);
            return this;
        }

        public Sprint Build ()
        {
            return new Sprint (Identity.Next (), _userStories, DateTime.Now, DateTime.Now);
        }
    }
}