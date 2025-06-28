using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoList.DataBase;
using ToDoList.Logger;
using ToDoList.Models;
using ToDoList.ViewModels;

namespace ToDoList.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserRepository _users;
        private readonly AppLogger _logger;

        public HomeController(UserRepository usersRepository, AppLogger appLogger)
        {
            _users = usersRepository;
            _logger = appLogger;
        }

        [Authorize]
        public async Task<IActionResult> Index(
            FilterViewModel filterViewModel,
            SortState sortState = SortState.DueDateAsc,
            int pageNumber = 0)
        {
            var currentUser = await GetCurrentUser();
            IEnumerable<TaskModel>? userTasks = currentUser.Tasks;

            filterViewModel ??= new FilterViewModel();
            var sortViewModel = new SortViewModel(sortState);
            var pageViewModel = new PageViewModel(userTasks.Count(), 1, userTasks.Count());

            if (userTasks.Any())
            {
                // Фильтрация
                userTasks = FilterTasks(userTasks, filterViewModel);

                //Сортировка
                userTasks = SortTasks(userTasks, sortViewModel);

                //Пагинация


                //int Compare(TaskModel a, TaskModel b)
                //{
                //    if (a.ExpiresDate == null)
                //        return -1;
                //    if (b.ExpiresDate == null)
                //        return 1;

                //    return (int)(a.ExpiresDate - b.ExpiresDate).TotalMinutes;
                //}
                //currentUser.Tasks.Sort(Compare);
            }




            IndexViewModel indexViewModel = new IndexViewModel(userTasks, pageViewModel, filterViewModel, sortViewModel);

            return View(indexViewModel);
        }

        [Authorize]
        public IActionResult EditTask(TaskModel task)
        {
            ValidateTask(() => (task == null || string.IsNullOrEmpty(task.Id)));
            return View(task);
        }

        [Authorize]
        public IActionResult CreateTask()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateTask(TaskModel task)
        {
            ValidateTask(() => (task == null));
            var user = await GetCurrentUser();
            await _users.AddTask(task, user);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateTask(TaskModel task)
        {
            ValidateTask(() => (task == null || string.IsNullOrEmpty(task.Id)));
            await _users.UpdateTask(task);
            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> DeleteTask(TaskModel task)
        {
            ValidateTask(() => (task == null || string.IsNullOrEmpty(task.Id)));
            await _users.DeleteTask(task);
            return RedirectToAction("Index");
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

        private IEnumerable<TaskModel> SortTasks(IEnumerable<TaskModel> tasks, SortViewModel sortViewModel)
        {
            tasks = sortViewModel.Current switch
            {
                SortState.LableAsc => tasks.OrderBy(t => t.Lable),
                SortState.LableDesc => tasks.OrderByDescending(t => t.Lable),
                SortState.DueDateAsc => tasks.OrderBy(t => t.ExpiresDate),
                SortState.DueDateDesc => tasks.OrderByDescending(t => t.ExpiresDate),
                SortState.StatusAsc => tasks.OrderBy(t => t.Status),
                SortState.StatusDesc => tasks.OrderByDescending(t => t.Status),
                SortState.PriorityAsc => tasks.OrderBy(t => t.Priority),
                SortState.PriorityDesc => tasks.OrderByDescending(t => t.Priority),
                _ => throw new NotImplementedException(),
            };
            return tasks;
        }

        private IEnumerable<TaskModel> FilterByDate(IEnumerable<TaskModel> userTasks, ViewDateScope dateScope)
        {
            var now = DateTime.Now;

            userTasks = dateScope switch
            {
                ViewDateScope.Today =>
                   userTasks = userTasks.Where(t => t.ExpiresDate.Date == now.Date),
                ViewDateScope.Tomorrow =>
                    userTasks = userTasks.Where(t => t.ExpiresDate.Date == now.AddDays(1).Date),
                ViewDateScope.ThisMonth =>
                    userTasks.Where(t => (t.ExpiresDate.Year == now.Year) && (t.ExpiresDate.Month == now.Month)),
                ViewDateScope.All => userTasks,
                _ => throw new NotImplementedException(),
            };
            return userTasks;
        }

        private async Task<UserModel> GetCurrentUser()
        {
            var userInApp = HttpContext.User.Identity;
            var currentUser = await _users.GetByEmail(userInApp.Name);
            return currentUser;
        }

        private void ValidateTask(Func<bool> condition)
        {
            if (condition.Invoke())
            {
                _logger.LogError("Task is null or incorrect.");
                throw new ArgumentNullException("Task");
            }
        }
    }
}
