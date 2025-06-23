using Microsoft.VisualBasic;

namespace ToDoList.Models
{
    public class FilterModel
    {
        public string LablePart { get; }
        public TasksStatus Status { get; }
        public DueDate DueDate { get; }
    }
}
