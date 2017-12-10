using System;

namespace AgileApp.Domain.Entities
{
    public class InvalidUserStoryStateException : Exception
    {
        public InvalidUserStoryStateException (UserStoryState currentState, UserStoryState invalaidState) : base (
            $"UserStory cannot move from {currentState} to {invalaidState}")
        { }
    }
}