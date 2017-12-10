using System;

namespace AgileApp.Framework
{
    public static class ActionExtensions 
    {
        public static Action<T> StartWith<T>(this Action<T> action, Action<T> preceedingAction)
        {
            return preceedingAction + action.EmptyIfNull();
        }

        public static Action<T> EmptyIfNull<T>(this Action<T> action)
        {
            return action ?? Empty<T>;
        }

        public static void Empty<T>(T arg)
        {
            
        }
    }
}