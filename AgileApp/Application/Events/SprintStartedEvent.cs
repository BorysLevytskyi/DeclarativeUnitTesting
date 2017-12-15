namespace AgileApp.Application.Events
{
    public class SprintStartedEvent
    {
        public int SprintId { get; set; }
        
        public int StartedByUserId { get; set; }
        
        public string StartedByUserName { get; set; }
    }
}