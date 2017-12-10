using System;
using AgileApp.Domain.Entities;

namespace AgileApp.Tests.Builders
{
    public class UserBuilder
    {
        private readonly User _user;

        public UserBuilder (User user)
        {
            _user = user;
        }

        public UserBuilder Email (string email)
        {
            _user.Email = email;
            return this;
        }

        public UserBuilder Named (string name)
        {
            _user.Name = name;
            return this;
        }

        public UserBuilder HasAccessTo (Sprint sprint)
        {
            _user.AllowedSprintIds.Add (sprint.Id);
            return this;
        }
    }
}