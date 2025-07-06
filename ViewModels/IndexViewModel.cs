using ToDoList.Models;

namespace ToDoList.ViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<TaskModel> Tasks { get; }
        public PageViewModel PageViewModel { get; }
        public FilterViewModel FilterViewModel { get; }
        public SortViewModel SortViewModel { get; }
        public bool IsAdmin { get; }
        public bool CanEditTasks { get; }
        public IndexViewModel(IEnumerable<TaskModel> tasks, PageViewModel pageViewModel,
            FilterViewModel filterViewModel, SortViewModel sortViewModel, bool isAdmin, bool canEditTasks)
        {
            Tasks = tasks;
            PageViewModel = pageViewModel;
            FilterViewModel = filterViewModel;
            SortViewModel = sortViewModel;
            IsAdmin = isAdmin;
            CanEditTasks = canEditTasks;
        }
    }
}
