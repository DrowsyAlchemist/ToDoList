using Microsoft.AspNetCore.Mvc.Rendering;
namespace ToDoList.Models
{
    public class FilterModel
    {
        public string? LablePart { get; }
        public TasksStatus? Status { get; }
        public ViewDateScope ViewDateScope { get; } = ViewDateScope.All;
        public Priority? Priority { get; }
    }
}
