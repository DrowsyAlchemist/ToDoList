using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoList.DataBase;
using ToDoList.Logger;
using ToDoList.Models;
using ToDoList.Services;

namespace ToDoList.Controllers
{
    public class TaskController : Controller
    {
        private readonly UserRepository _users;
        private readonly AppLogger _logger;
        private readonly ValidationMessageMaker _validationMessageMaker;
        private readonly TaskNotifier _notifier;

        public TaskController(UserRepository usersRepository,
            AppLogger appLogger,
            ValidationMessageMaker validationMessageMaker,
            TaskNotifier notifier)
        {
            _users = usersRepository;
            _logger = appLogger;
            _validationMessageMaker = validationMessageMaker;
            _notifier = notifier;
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
                _notifier.AddNotification(task);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Error", new { message = ex.Message, stackTrace = ex.StackTrace });
            }
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public IActionResult EditTask(TaskModel task)
        {
            ValidateTask(() => (task == null || string.IsNullOrEmpty(task.Id)));
            return View(task);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateTask(TaskModel task)
        {
            if (ModelState.IsValid == false)
                return ViewValidationError("EditTask", task);

            await _users.UpdateTask(task);
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> DeleteTask(TaskModel task)
        {
            ValidateTask(() => (task == null || string.IsNullOrEmpty(task.Id)));
            await _users.DeleteTask(task);
            return RedirectToAction("Index", "Home");
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
