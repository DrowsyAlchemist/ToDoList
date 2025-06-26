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
            FilterModel? filterModel = null,
            SortState sortState = SortState.DueDateAsc,
            int pageNumber = 0)
        {
            /////////////////////////////////////////////////////////////////////////

            var currentUser = await GetCurrentUser();
            IEnumerable<TaskModel>? userTasks = currentUser.Tasks;

            if (userTasks.Any() && filterModel != null)
            {
                if (string.IsNullOrEmpty(filterModel.LablePart) == false)
                    userTasks = userTasks.Where(t => t.Lable.Contains(filterModel.LablePart));

                if (filterModel.Status != null)
                    userTasks = userTasks.Where(t => t.Status == filterModel.Status);

                if (filterModel.Priority != null)
                    userTasks = userTasks.Where(t => t.Priority == filterModel.Priority);

                if (filterModel.ViewDateScope != ViewDateScope.All)
                {
                    var now = DateTime.Now;

                    if (filterModel.ViewDateScope == ViewDateScope.Today)
                        userTasks = userTasks.Where(t => t.ExpiresDate.Date == now.Date);

                    if (filterModel.ViewDateScope == ViewDateScope.Tomorrow)
                        userTasks = userTasks.Where(t => t.ExpiresDate.Date == now.AddDays(1));

                    if (filterModel.ViewDateScope == ViewDateScope.ThisMonth)
                        userTasks = userTasks.Where(t => (t.ExpiresDate.Year == now.Year) && (t.ExpiresDate.Month == now.Month));
                }
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

            FilterViewModel filterViewModel = new FilterViewModel();
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
