namespace AgileApp.Domain.Entities
{
    public class Assignee
    {
        public Assignee()
        {
        }

        public Assignee (int userId, string name, string email)
        {
            UserId = userId;
            Name = name;
            Email = email;
        }

        public int UserId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }
    }
}