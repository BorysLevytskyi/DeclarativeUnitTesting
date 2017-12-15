using System;
using System.Collections.Generic;
using System.Text;
using AgileApp.Application.Entities;

namespace AgileApp.Tests.Builders
{
    public class SprintBuilder
    {
        private readonly List<UserStory> _userStories = new List<UserStory> ();
        
        private Sprint _sprint;

        public SprintBuilder()
        {
            _sprint = new Sprint();    
        }
        
        public SprintBuilder UserStory(string title)
        {
            return UserStory(u => u.Title(title));
        }
        
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

        public SprintBuilder Starts(DateTime startDate)
        {
            _sprint.StartDate = startDate;
            return this;
        }

        public SprintBuilder Ends(DateTime endDate)
        {
            _sprint.EndDate = endDate;
            return this;
        }
    }
}