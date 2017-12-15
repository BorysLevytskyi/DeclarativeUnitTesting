using System;
using AgileApp.Application.Entities;

namespace AgileApp.Application.Exceptions
{
    public class InvalidUserStoryStateException : Exception
    {
        public InvalidUserStoryStateException (UserStoryState currentState, UserStoryState invalaidState) : base (
            $"UserStory cannot move from {currentState} to {invalaidState}")
        { }
    }
}