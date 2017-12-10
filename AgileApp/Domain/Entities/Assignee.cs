namespace AgileApp.Domain.Entities
{
    public class Assignee
    {

        public Assignee(int userId, string name, string email)
        {
            UserId = userId;
            Name = name;
            Email = email;
        }

        public int UserId { get; }

        public string Name { get; }
        
        public string Email { get; }
    }
}