using System;
using System.Collections.Generic;
using System.Linq;

namespace AgileApp.Domain.Entities
{
    public class Sprint
    {
        public Sprint(int id, IEnumerable<UserStory> userStories, DateTime startDate, DateTime endDate)
        {
            Id = id;
            UserStories = userStories.ToList();
            StartDate = startDate;
            EndDate = endDate;
        }

        public DateTime StartDate { get; }

        public DateTime EndDate { get; }

        public IReadOnlyList<UserStory> UserStories { get; }

        public StoryPoint TotalEstimate => new StoryPoint(UserStories.Where(us => us.Estimate.HasValue).Sum(us => us.Estimate.Value.Value));

        public int Id { get; private set; }

        public void Start()
        {
            if (!UserStories.Any())
            {
                throw new EmptyStrintException();
            }
        }

        public void Complete()
        {

        }
    }

    public class EmptyStrintException : Exception
    {
    }
}