namespace Domain.Entities
{
    public struct StoryPoint
    {
        public int Value { get; }

        public StoryPoint(int value)
        {
            Value = value;
        }
    }

    public static class StoryPointExtensions
    {
        public static StoryPoint StoryPoints(this int number)
        {
            return new StoryPoint(number);
        }
    }
}