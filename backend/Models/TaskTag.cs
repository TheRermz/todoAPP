namespace todoApp.Models
{
    public class TaskTag
    {
        //FK
        public int TodoTaskId { get; set; }
        public int TagId { get; set; }

        // Navigation Props
        public TodoTask? TodoTask { get; set; }
        public Tag? Tag { get; set; }
    }
}
