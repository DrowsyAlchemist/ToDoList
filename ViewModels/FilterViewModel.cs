using Microsoft.AspNetCore.Mvc.Rendering;
using ToDoList.Models;

namespace ToDoList.ViewModels
{
    public class FilterViewModel
    {
        public string LablePart { get; set; } = string.Empty;

        public SelectList Data { get; set; }
        public ViewDateScope SelectedDateScope { get; set; } = ViewDateScope.All;

        public SelectList Status { get; set; }
        public TasksStatus? SelectedStatus { get; set; }

        public SelectList Priority { get; set; }
        public Priority? SelectedPriority { get; set; }

        public FilterViewModel()
        {
            string[] viewDateScopes = Enum.GetNames(typeof(ViewDateScope));
            var viewDateScopesList = viewDateScopes.ToList();
            Data = new SelectList(viewDateScopesList);

            string[] tasksStatuses = Enum.GetNames(typeof(TasksStatus));
            var tasksStatusesList = tasksStatuses.ToList();
            tasksStatusesList.Insert(0, "All");
            Status = new SelectList(tasksStatusesList);

            string[] priorities = Enum.GetNames(typeof(Priority));
            var prioritiesList = priorities.ToList();
            prioritiesList.Insert(0, "All");
            Priority = new SelectList(prioritiesList);
        }
    }
}
