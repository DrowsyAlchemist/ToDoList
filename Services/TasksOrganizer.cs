using ToDoList.Models;
using ToDoList.ViewModels;

namespace ToDoList.Services
{
    public class TasksOrganizer
    {
        public IndexViewModel Organize(
            IEnumerable<TaskModel> userTasks,
            TasksOrganizationInfo organizationInfo,
            UserViewModel userViewModel)
        {
            if (organizationInfo.FilterViewModel == null)
                organizationInfo.FilterViewModel = new FilterViewModel();

            if (organizationInfo.PageViewModel == null)
                organizationInfo.PageViewModel = new PageViewModel();

            if (userTasks.Any())
            {
                userTasks = FilterTasks(userTasks, organizationInfo.FilterViewModel);
                userTasks = SortTasks(userTasks, organizationInfo.SortState);

                organizationInfo.PageViewModel.ItemsCount = userTasks.Count();
                userTasks = Paginate(userTasks, organizationInfo.PageViewModel);
            }
            return new IndexViewModel(userTasks, organizationInfo, userViewModel);
        }

        private IEnumerable<TaskModel> FilterTasks(IEnumerable<TaskModel> tasks, FilterViewModel filterViewModel)
        {
            if (string.IsNullOrEmpty(filterViewModel.LablePart) == false)
                tasks = tasks.Where(t => t.Lable.Contains(filterViewModel.LablePart));

            if (filterViewModel.SelectedStatus != null)
                tasks = tasks.Where(t => t.Status == filterViewModel.SelectedStatus);

            if (filterViewModel.SelectedPriority != null)
                tasks = tasks.Where(t => t.Priority == filterViewModel.SelectedPriority);

            if (filterViewModel.SelectedDateScope != ViewDateScope.All)
                tasks = FilterByDate(tasks, filterViewModel.SelectedDateScope);

            return tasks;
        }

        private IEnumerable<TaskModel> SortTasks(IEnumerable<TaskModel> tasks, SortState sortState)
        {
            tasks = sortState switch
            {
                SortState.LableAsc => tasks.OrderBy(t => t.Lable),
                SortState.LableDesc => tasks.OrderByDescending(t => t.Lable),
                SortState.DueDateAsc => tasks.OrderBy(t => t.ExpiresDate),
                SortState.DueDateDesc => tasks.OrderByDescending(t => t.ExpiresDate),
                SortState.PriorityAsc => tasks.OrderBy(t => t.Priority),
                SortState.PriorityDesc => tasks.OrderByDescending(t => t.Priority),
                _ => throw new NotImplementedException(),
            };
            return tasks;
        }

        private IEnumerable<TaskModel> Paginate(IEnumerable<TaskModel> tasks, PageViewModel pageViewModel)
        {
            int skipCount = (pageViewModel.PageNumber - 1) * pageViewModel.ItemsPerPage;

            if (skipCount >= tasks.Count())
            {
                pageViewModel.PageNumber = 1;
                skipCount = 0;
            }
            tasks = tasks.Skip(skipCount).Take(pageViewModel.ItemsPerPage);
            return tasks;
        }

        private IEnumerable<TaskModel> FilterByDate(IEnumerable<TaskModel> tasks, ViewDateScope dateScope)
        {
            var now = DateTime.Now;

            tasks = dateScope switch
            {
                ViewDateScope.Today =>
                   tasks = tasks.Where(t => t.ExpiresDate.Date == now.Date),
                ViewDateScope.Tomorrow =>
                    tasks = tasks.Where(t => t.ExpiresDate.Date == now.AddDays(1).Date),
                ViewDateScope.ThisMonth =>
                    tasks.Where(t => (t.ExpiresDate.Year == now.Year) && (t.ExpiresDate.Month == now.Month)),
                ViewDateScope.All => tasks,
                _ => throw new NotImplementedException(),
            };
            return tasks;
        }
    }
}