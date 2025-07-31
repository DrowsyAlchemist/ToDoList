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

        public HomeController(UserRepository usersRepository,
            AppLogger appLogger,
            TasksOrganizer tasksOrganizer,
            TaskNotifier notifier)
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
    }
}
