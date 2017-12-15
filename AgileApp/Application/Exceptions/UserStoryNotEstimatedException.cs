using System;

namespace AgileApp.Application.Exceptions
{
    public class UserStoryNotEstimatedException : Exception
    {
        public UserStoryNotEstimatedException () : base ("User story is not estimated")
        { }
    }
}