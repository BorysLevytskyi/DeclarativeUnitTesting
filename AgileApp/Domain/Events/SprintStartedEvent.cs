using System;
using AgileApp.Domain.Entities;

namespace AgileApp.Domain.Events
{
    public class SprintStartedEvent
    {
        public Sprint Sprint { get; }
        public DateTimeOffset StartedDate { get; }
        public SprintStartedEvent (Sprint sprint, DateTimeOffset startedDate)
        {
            this.StartedDate = startedDate;
            this.Sprint = sprint;

        }
    }
}