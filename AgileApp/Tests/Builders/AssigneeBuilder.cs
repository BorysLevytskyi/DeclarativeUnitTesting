namespace AgileApp.Tests.Builders
{
    public class AssigneeBuilder
    {
        private string _name;
        private string _email;
        private int _id;

        public AssigneeBuilder ()
        { }

        public AssigneeBuilder Named (string name)
        {
            _name = name;
            return this;
        }

        public AssigneeBuilder WithEmail (string email)
        {

        }
    }
}