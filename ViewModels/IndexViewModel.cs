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

        public IndexViewModel(IEnumerable<TaskModel> tasks, TasksOrganizationInfo organizationInfo, UserViewModel userViewModel)
        {
            if (organizationInfo.FilterViewModel == null)
                throw new ArgumentNullException(nameof(organizationInfo.FilterViewModel));

            if (organizationInfo.PageViewModel == null)
                throw new ArgumentNullException(nameof(organizationInfo.PageViewModel));

            Tasks = tasks;
            FilterViewModel = organizationInfo.FilterViewModel;
            SortViewModel = new SortViewModel(organizationInfo.SortState);
            PageViewModel = organizationInfo.PageViewModel;
            UserViewModel = userViewModel;
        }
    }
}
