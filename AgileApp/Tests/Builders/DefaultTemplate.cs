namespace AgileApp.Tests.Builders
{
    public class DefaultTemplate
    {
        public static void User (UserBuilder user)
        {
            user.Named ("John Smith")
                .Email ("jon.smith@corp.com")
                .Active ()
                .HasARoleOf (RoleType.SysAdmin);
        }
        public static void UserStory (UserStoryBuilder userStory)
        {
            userStory.Id (Identity.Next ()) // Generarte unique int id
                .Unassigned ()
                .Unestimated ()
                .Title ($"Generic story #{id}");
        }
    }
}