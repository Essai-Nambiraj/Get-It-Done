namespace Get_It_Done.Models
{
    public class TaskItem
    {
        //public int UserId { get; set; }
        public int TaskId { get; set; }
        public string Task { get; set; }

        public bool IsCompleted { get; set; }
        public bool IsPriority { get; set; }

        public DateTime? DeadLine { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public List<TaskItem> Tasks { get; set; }

        public string CurrentFilter { get; set; }   //All Completed, Pending, Priority, Trash

        public bool IsCompleteAll { get; set; }
        public bool IsDeleteAll { get; set; }


    }
}
