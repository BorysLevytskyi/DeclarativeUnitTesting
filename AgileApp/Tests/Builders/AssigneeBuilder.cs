using AgileApp.Application.Entities;

namespace AgileApp.Tests.Builders
{
    public class AssigneeBuilder
    {

        private Assignee _assignee;

        public AssigneeBuilder()
        {
            _assignee = new Assignee {UserId = Identity.Next()};
        }

        public AssigneeBuilder Named(string name)
        {
            _assignee.Name = name;
            return this;
        }

        public AssigneeBuilder WithEmail(string email)
        {
            _assignee.Email = email;
            return this;
        }

        public Assignee Build()
        {
            return _assignee;
        }
    }
}