namespace Domain.Entities
{
    public class UserStory
    {
        private readonly UserStoryData _data;

        public UserStory(string title)
        {
            _data = new UserStoryData
            {
                Title = title,
                State = UserStoryState.InBacklog
            };
        }

        private UserStory(UserStoryData data)
        {
            _data = data;
        }

        public int Id => _data.Id;

        public StoryPoint? Estimate => _data.Estimate;

        public string Title => _data.Title;

        public Assignee Assignee => _data.Assignee;

        public UserStoryState State => _data.State;

        public static UserStory Reconstitute(UserStoryData data)
        {
            return new UserStory(data);
        }

        public void SetEstimate(StoryPoint estimate)
        {
            _data.Estimate = estimate;
        }

        public void Assign(Assignee assignee)
        {
            _data.Assignee = assignee;
        }

        public void Schedule()
        {

        }

        public void Start()
        {}

        public void Complete()
        {

        }

        public void Accept()
        {

        }

        public void Reject()
        {

        }

        public string ToShortString()
        {
            return $"{Title} @{Assignee.Name}";
        }

        public UserStoryData GetData()
        {
            return _data;
        }
    }

    public class UserStoryData
    {
        public Assignee Assignee { get; set; }

        public UserStoryState State { get; set; }

        public StoryPoint? Estimate { get; set; }

        public string Title { get; set; }

        public int Id { get; set; }
    }
}