namespace todoApp.Models
{
    public enum Status
    {
        Pending,
        InProgress,
        Completed,
        Late
    }

    public enum Priority
    {
        Low,
        Medium,
        High
    }


    public class TodoTask
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public Status Status { get; set; } = Status.Pending;
        public Priority Priority { get; set; } = Priority.Low;
        public List<TaskTag> TaskTags { get; set; } = new();

        // FK

        public int UserId { get; set; }

        // Navigation Prop
        public User? User { get; set; }
    }
}
