using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AgileApp.Application.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public ICollection<RoleType> Roles { get; set; } = new Collection<RoleType> ();

        public ICollection<int> AllowedSprintIds { get; set; } = new Collection<int> ();
        
        public bool IsActive { get; set; }
    }
}