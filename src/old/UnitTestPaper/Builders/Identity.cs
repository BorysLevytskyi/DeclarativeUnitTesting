namespace Tests.Builders
{
    public static class Identity
    {
        private static int _id = 1;

        public static int Next()
        {
            return _id++;
        }
    }
}