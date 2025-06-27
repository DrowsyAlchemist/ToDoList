namespace ToDoList.Models
{
    public class FilterModel
    {
        public string? LablePart { get; set; }
        public TasksStatus? Status { get; set; }
        public ViewDateScope ViewDateScope { get; set; } = ViewDateScope.All;
        public Priority? Priority { get; set; }
    }
}
