using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
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

            if (userTasks.Any())
            {
                if (filterViewModel != null)
                {
                    if (string.IsNullOrEmpty(filterViewModel.LablePart) == false)
                        userTasks = userTasks.Where(t => t.Lable.Contains(filterViewModel.LablePart));

                    if (filterViewModel.SelectedStatus != null)
                        userTasks = userTasks.Where(t => t.Status == filterViewModel.SelectedStatus);

                    if (filterViewModel.SelectedPriority != null)
                        userTasks = userTasks.Where(t => t.Priority == filterViewModel.SelectedPriority);

                    if (filterViewModel.SelectedDateScope != ViewDateScope.All)
                    {
                        var now = DateTime.Now;

                        if (filterViewModel.SelectedDateScope == ViewDateScope.Today)
                            userTasks = userTasks.Where(t => t.ExpiresDate.Date == now.Date);

                        if (filterViewModel.SelectedDateScope == ViewDateScope.Tomorrow)
                            userTasks = userTasks.Where(t => t.ExpiresDate.Date == now.AddDays(1).Date);

                        if (filterViewModel.SelectedDateScope == ViewDateScope.ThisMonth)
                            userTasks = userTasks.Where(t => (t.ExpiresDate.Year == now.Year) && (t.ExpiresDate.Month == now.Month));
                    }
                }
                else
                {
                    filterViewModel = new FilterViewModel();
                }

                int Compare(TaskModel a, TaskModel b)
                {
                    if (a.ExpiresDate == null)
                        return -1;
                    if (b.ExpiresDate == null)
                        return 1;

                    return (int)(a.ExpiresDate - b.ExpiresDate).TotalMinutes;
                }
                currentUser.Tasks.Sort(Compare);
            }

            SortViewModel sortViewModel = new SortViewModel(SortState.LableAsc);
            PageViewModel pageViewModel = new PageViewModel(userTasks.Count(), 1, userTasks.Count());

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
