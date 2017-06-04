using System;
using Domain.Entities;

namespace Tests.Builders
{
    public class UserBuilder
    {
        private readonly User _user;

        public UserBuilder(User user)
        {
            _user = user;
        }

        public void Named(string name)
        {
            _user.Name = name;
        }
    }

    public class AssigneeBuilder 
    {
        private string _name;
        private string _email;
        private int _id;
        
        public AssigneeBuilder()
        {
        }

        public AssigneeBuilder Named(string name)
        {
            _name = name;
            return this;
        }

        public AssigneeBuilder WithEmail(string email)
        {
            
        }
    }
}
