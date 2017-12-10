using System;

namespace AgileApp.Domain.Exceptions
{
    public class UserStoryNotEstimatedException : Exception
    {
        public UserStoryNotEstimatedException () : base ("User story is not estimated")
        { }
    }
}