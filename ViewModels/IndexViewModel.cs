using ToDoList.Models;

namespace ToDoList.ViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<TaskModel> Tasks { get; }
        public PageViewModel PageViewModel { get; }
        public FilterViewModel FilterViewModel { get; }
        public SortViewModel SortViewModel { get; }
        public UserViewModel UserViewModel { get; }

        public IndexViewModel(IEnumerable<TaskModel> tasks, PageViewModel pageViewModel,
            FilterViewModel filterViewModel, SortViewModel sortViewModel, UserViewModel userViewModel)
        {
            Tasks = tasks;
            PageViewModel = pageViewModel;
            FilterViewModel = filterViewModel;
            SortViewModel = sortViewModel;
            UserViewModel = userViewModel;
        }
    }
}
