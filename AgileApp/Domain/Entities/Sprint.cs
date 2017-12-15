using System;
using System.Collections.Generic;
using System.Linq;

namespace AgileApp.Domain.Entities
{
    public class Sprint
    {
        public Sprint()
        {
        }
        
        public Sprint (int id, IEnumerable<UserStory> userStories, DateTime startDate, DateTime endDate)
        {
            Id = id;
            UserStories = userStories.ToList ();
            StartDate = startDate;
            EndDate = endDate;
        }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public ICollection<UserStory> UserStories { get; set; } = new List<UserStory>();

        public int Id { get; set; }

        public StoryPoint CalculateTotalEstimate()
        {
            return new StoryPoint(UserStories.Where(us => us.Estimate.HasValue).Sum(us => us.Estimate.Value.Value));
        }

        public void Start ()
        {
            if (!UserStories.Any ())
            {
                throw new EmptyStrintException ();
            }
        }

        public void Complete ()
        {
        }

        public StoryPoint CalculateTotalEstimateFor(int userId)
        {
            throw new NotImplementedException();
        }
    }

    public class EmptyStrintException : Exception
    { }
}