using System;
using AgileApp.Domain.Entities;

namespace AgileApp.Domain.Events
{
    public class SprintStartedEvent
    {
        public int SprintId { get; set; }
        
        public int StartedByUserId { get; set; }
        
        public string StartedByUserName { get; set; }
    }
}