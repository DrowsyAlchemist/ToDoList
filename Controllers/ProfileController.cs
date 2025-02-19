using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoList.DataBase;
using ToDoList.Logger;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserRepository _users;
        private readonly AppLogger _logger;

        public ProfileController(UserRepository userRepository, AppLogger logger)
        {
            _users = userRepository;
            _logger = logger;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = HttpContext.User;

            if (user == null || user.Identity == null || user.Identity.IsAuthenticated == false)
            {
                _logger.LogError("User is not found or is not authenticated");
                return Unauthorized();
            }

            string? email = user.Identity.Name;

            if (email == null)
            {
                _logger.LogError("User's email is null.");
                return Unauthorized();
            }
            var currentUser = await _users.GetByEmail(email);

            if (currentUser == null)
            {
                _logger.LogError($"User with email \"{email}\" is not found.");
                return Unauthorized();
            }
            return View(currentUser);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Index(UserModel editedUser)
        {
            await _users.UpdateUser(editedUser);
            return View(editedUser);
        }
    }
}
