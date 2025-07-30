using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ToDoList.DataBase;
using ToDoList.Logger;
using ToDoList.Models;
using ToDoList.Services;
using ToDoList.SignalR;
using ToDoList.ViewModels;

namespace ToDoList.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserRepository _users;
        private readonly AppLogger _logger;
        private readonly TasksOrganizer _tasksOrganizer;
        private readonly NotificationHub _hubContext;
        private readonly ValidationMessageMaker _validationMessageMaker;

        public HomeController(UserRepository usersRepository,
            AppLogger appLogger,
            TasksOrganizer tasksOrganizer,
            IHubContext<NotificationHub> hubContext,
            ValidationMessageMaker validationMessageMaker)
        {
            _users = usersRepository;
            _logger = appLogger;
            _tasksOrganizer = tasksOrganizer;
            _hubContext = hubContext as NotificationHub;
            _validationMessageMaker = validationMessageMaker;
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

            if (userToView == null)
                return RedirectToAction("Index", "Error", new { message = "UserToView is not found." });

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

            int? itemsPerPage = organizationInfo.PageViewModel?.ItemsPerPage;
            organizationInfo.PageViewModel = new PageViewModel();

            if (itemsPerPage != null)
                organizationInfo.PageViewModel.ItemsPerPage = itemsPerPage.Value;

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
            if (ModelState.IsValid == false)
                return ViewValidationError("CreateTask", task);

            var user = await GetCurrentUser();

            try
            {
                await _users.AddTask(task, user);
                SendNotification(task);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Error", new { message = ex.Message, stackTrace = ex.StackTrace });
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateTask(TaskModel task)
        {
            if (ModelState.IsValid == false)
                return ViewValidationError("EditTask", task);

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

        private void SendNotification(TaskModel task)
        {
            var delay = task.ExpiresDate - DateTime.Now;

            if (delay > TimeSpan.Zero)
            {
                _ = Task.Delay(delay).ContinueWith(async (_) =>
                 {
                     if (_hubContext.IsUserOnline(task.User.LoginData.Email))
                         await SendBrowserNotification(task);
                     else
                         SendEmailNotification(task);
                 });
            }
        }

        private async Task SendBrowserNotification(TaskModel task)
        {
            await _hubContext.Clients.Group(task.User.LoginData.Email)
                .SendAsync("ReceiveNotification",
                $"Время выполнить задачу: {task.Lable}",
                task.Id);
        }

        private async Task SendEmailNotification(TaskModel task)
        {
            throw new NotImplementedException();
        }

        private ViewResult ViewValidationError(string viewName, TaskModel task)
        {
            string errorMessages = _validationMessageMaker.GetValidationErrorMessage(ModelState);
            return ViewWarning(viewName, errorMessages, task: task);
        }

        private ViewResult ViewWarning(string viewName, string message, string? logMessage = null, TaskModel? task = null)
        {
            if (logMessage != null)
                _logger.LogWarning(logMessage);

            ViewBag.ErrorMessage = message;
            return View(viewName, task);
        }

        private async Task<UserModel> GetCurrentUser()
        {
            var userInApp = HttpContext.User.Identity;

            if (userInApp == null)
            {
                _logger.LogError("HomeController/GetCurrentUser: User is null.");
                throw new InvalidOperationException("User is null.");
            }
            var userInDb = await _users.GetByEmail(userInApp.Name);

            if (userInDb == null)
            {
                _logger.LogError("HomeController/GetCurrentUser: UserInDb is null.");
                throw new InvalidOperationException("UserInDb is null.");
            }
            return userInDb;
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
