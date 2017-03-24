using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Entities
{
    public class Sprint
    {
        public Sprint(IEnumerable<UserStory> userStories, DateTime startDate, DateTime endDate)
        {
            UserStories = userStories.ToList();
            StartDate = startDate;
            EndDate = endDate;
        }

        public DateTime StartDate { get; }

        public DateTime EndDate { get; }

        public IReadOnlyList<UserStory> UserStories { get; }

        public StoryPoint TotalEstimate => new StoryPoint(UserStories.Where(us => us.Estimate.HasValue).Sum(us => us.Estimate.Value.Value));

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