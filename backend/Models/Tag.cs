namespace todoApp.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public List<TaskTag> TaskTags { get; set; } = new();
    }
}
