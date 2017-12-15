using AgileApp.Application.Entities;

namespace AgileApp.Tests.Builders
{
    public class UserBuilder
    {
        private readonly User _user = new User();

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

        public UserBuilder HasAccessTo(Sprint sprint)
        {
            _user.AllowedSprintIds.Add(sprint.Id);
            return this;
        }

        public UserBuilder Active()
        {
            _user.IsActive = true;
            return this;
        }

        public UserBuilder HasARoleOf(RoleType role)
        {
            _user.Roles.Add(role);
            return this;
        }

        public User Build() => _user;
    }
}