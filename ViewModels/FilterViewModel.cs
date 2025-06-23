using Microsoft.AspNetCore.Mvc.Rendering;
using ToDoList.Models;

namespace ToDoList.ViewModels
{
    public class FilterViewModel
    {
        public string LablePart { get; set; }

        public SelectList Data { get; set; }
        public ViewDateScope SelectedDateScope { get; set; }

        public SelectList Status { get; set; }
        public Models.TasksStatus SelectedStatus { get; set; }

        public SelectList Priority { get; set; }
        public Priority SelectedPriority { get; set; }
    }
}
