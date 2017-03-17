using System;

namespace Domain.Entities
{
    public class InvalidUserStoryStateException : Exception
    {
        public InvalidUserStoryStateException(UserStoryState currentState, UserStoryState invalaidState) : base(
            $"UserStory cannot move from {currentState} to {invalaidState}")
        {
        }
    }
}