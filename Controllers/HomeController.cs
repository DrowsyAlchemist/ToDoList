using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoList.DataBase;
using ToDoList.Logger;
using ToDoList.Models;
using ToDoList.Services;
using ToDoList.ViewModels;

namespace ToDoList.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserRepository _users;
        private readonly AppLogger _logger;
        private readonly TasksOrganizer _tasksOrganizer;

        public HomeController(UserRepository usersRepository, AppLogger appLogger, TasksOrganizer tasksOrganizer)
        {
            _users = usersRepository;
            _logger = appLogger;
            _tasksOrganizer = tasksOrganizer;
        }

        [Authorize]
        public async Task<IActionResult> Index(TasksOrganizationInfo organizationInfo, UserViewModel userViewModel)
        {
            UserModel currentUser = await GetCurrentUser();
            UserModel userToView;

            if (string.IsNullOrEmpty(userViewModel.UserId) == false)
                userToView = await _users.GetById(userViewModel.UserId);
            else
                userToView = currentUser;

            IEnumerable<TaskModel>? userTasks = userToView.Tasks;

            userViewModel.IsAdminMode = currentUser.Role == Role.Admin;
            userViewModel.IsUserTasksOwner = currentUser.Id.Equals(userToView.Id);

            if (userViewModel.IsUserTasksOwner == false)
            {
                if (userViewModel.IsAdminMode == false)
                    throw new InvalidOperationException();

                userViewModel.UserId = userToView.Id;
                userViewModel.Name = userToView.Name;
                userViewModel.Email = userToView.LoginData.Email;
            }
            var indexViewModel = _tasksOrganizer.Organize(userTasks, organizationInfo, userViewModel);
            return View("Index", indexViewModel);
        }

        public async Task<IActionResult> ResetFilter(TasksOrganizationInfo organizationInfo, UserViewModel userViewModel)
        {
            organizationInfo.FilterViewModel = null;
            organizationInfo.PageViewModel = new PageViewModel { ItemsPerPage = organizationInfo.PageViewModel.ItemsPerPage };
            userViewModel = new UserViewModel { UserId = userViewModel.UserId };
            return await Index(organizationInfo, userViewModel);
        }

        [Authorize]
        public async Task<IActionResult> SetPageSize(TasksOrganizationInfo organizationInfo, int pageSize, UserViewModel userViewModel)
        {
            if (pageSize > 0)
                organizationInfo.PageViewModel = new PageViewModel { ItemsPerPage = pageSize };

            return await Index(organizationInfo, userViewModel);
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
