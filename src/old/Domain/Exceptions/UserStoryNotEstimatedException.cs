using System;

namespace Domain.Exceptions
{
    public class UserStoryNotEstimatedException : Exception
    {
        public UserStoryNotEstimatedException() : base("User story is not estimated")
        {
        }
    }
}