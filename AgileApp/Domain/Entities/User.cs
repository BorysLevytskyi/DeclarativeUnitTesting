using System;
using System.Collections.Generic;

namespace AgileApp.Domain.Entities
{
    public class User
    {
        public User ()
        { }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public ICollection<RoleType> Roles { get; set; } = new Collection<RoleType> ();

        public ICollection<int> AllowedSprintIds { get; set; } = new Collection<int> ();
    }
}