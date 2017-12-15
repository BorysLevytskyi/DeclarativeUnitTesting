using System;
using System.Collections.Generic;
using System.Linq;
using AgileApp.Application.Exceptions;

namespace AgileApp.Application.Entities
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

        public void Start()
        {
            if (!UserStories.Any ())
            {
                throw new EmptySprintException ();
            }
        }

        public void Complete()
        {
        }

        public int CalculateTotalEstimate()
        {
            return UserStories.Sum(us => us.Estimate ?? 0);
        }

        public int CalculateTotalEstimateFor(int userId)
        {
            return UserStories.Where(s => s.Assignee.UserId == userId).Sum(s => s.Estimate ?? 0);
        }
    }
}