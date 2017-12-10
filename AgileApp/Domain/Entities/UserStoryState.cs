namespace AgileApp.Domain.Entities
{
    public enum UserStoryState
    {

        InBacklog,
        /// <summary>
        /// User story was added to sprint
        /// </summary>
        Scheduled,
        InProgress,
        Done,
        Accepted,
        Rejected
    }
}