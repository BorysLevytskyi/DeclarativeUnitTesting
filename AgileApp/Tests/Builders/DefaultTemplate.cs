using System;
using AgileApp.Application.Entities;

namespace AgileApp.Tests.Builders
{
    public static class DefaultTemplate
    {
        public static void User(UserBuilder user)
        {
            user.Named("John Smith")
                .Email("jon.smith@corp.com")
                .Active()
                .HasARoleOf(RoleType.Developer);
        }

        public static void Sprint(SprintBuilder sprint)
        {
            sprint.Starts(DateTime.Today).Ends(DateTime.Today.AddDays(14));
        }
       
        public static void UserStory(UserStoryBuilder userStory)
        {
            var id = Identity.Next ();
            userStory.Id(id) // Generarte unique int id
                .Unassigned()
                .Unestimated()
                .Title($"Generic story #{id}");
        }

        public static void Assignee(AssigneeBuilder assignee)
        {
            assignee.Named("John Smith").WithEmail("jothn.smipth@dummycorp.com");
        }
    }
}