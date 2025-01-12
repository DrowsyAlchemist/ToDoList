namespace ToDoList.Models
{
    public class TaskModel
    {
        public string? Id { get; set; }
        public string? Lable { get; set; }
        public DateTime? ExpiresDate { get; set; }
        public Status Status { get; set; }
        public Priority Priority { get; set; }
        public string? Description { get; set; }
    }
}
