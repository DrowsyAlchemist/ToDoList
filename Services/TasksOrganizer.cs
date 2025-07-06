using ToDoList.Models;
using ToDoList.ViewModels;

namespace ToDoList.Services
{
    public static class TasksOrganizer
    {
        private const int DefaultPageSize = 10;
        public static IndexViewModel Organize(
            IEnumerable<TaskModel>? userTasks,
            FilterViewModel? filterViewModel = null,
            SortState sortState = SortState.DueDateAsc,
            int pageNumber = 1,
            bool isAdmin = false,
            bool canEditTasks = false)
        {
            if (filterViewModel == null)
                filterViewModel = new FilterViewModel();

            var sortViewModel = new SortViewModel(sortState);
            var pageViewModel = new PageViewModel(userTasks.Count(), pageNumber, DefaultPageSize);

            if (userTasks.Any())
            {
                userTasks = FilterTasks(userTasks, filterViewModel);
                userTasks = SortTasks(userTasks, sortViewModel);

                pageViewModel = new PageViewModel(userTasks.Count(), pageNumber, DefaultPageSize);
                userTasks = Paginate(userTasks, pageViewModel);
            }
            return new IndexViewModel(userTasks, pageViewModel, filterViewModel, sortViewModel, isAdmin, canEditTasks);
        }

        private static IEnumerable<TaskModel> FilterTasks(IEnumerable<TaskModel> tasks, FilterViewModel filterViewModel)
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

        private static IEnumerable<TaskModel> SortTasks(IEnumerable<TaskModel> tasks, SortViewModel sortViewModel)
        {
            tasks = sortViewModel.Current switch
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

        private static IEnumerable<TaskModel> FilterByDate(IEnumerable<TaskModel> tasks, ViewDateScope dateScope)
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

        private static IEnumerable<TaskModel> Paginate(IEnumerable<TaskModel> tasks, PageViewModel pageViewModel)
        {
            int skipCount = (pageViewModel.PageNumber - 1) * pageViewModel.ItemsPerPage;
            tasks = tasks.Skip(skipCount).Take(pageViewModel.ItemsPerPage);
            return tasks;
        }
    }
}
