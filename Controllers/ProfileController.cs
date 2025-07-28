using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ToDoList.DataBase;
using ToDoList.Logger;
using ToDoList.Models;
using ToDoList.Services;

namespace ToDoList.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserRepository _users;
        private readonly AppLogger _logger;
        private readonly ValidationMessageMaker _validationMessageMaker;

        public ProfileController(UserRepository userRepository, AppLogger logger, ValidationMessageMaker validationMessageMaker)
        {
            _users = userRepository;
            _logger = logger;
            _validationMessageMaker = validationMessageMaker;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = HttpContext.User;

            if (user == null || user.Identity == null || user.Identity.IsAuthenticated == false)
                return RedirectToAction("Index", "Error", new { message = "User is not found or is not authenticated" });

            string? email = user.Identity.Name;

            if (email == null)
                return RedirectToAction("Index", "Error", new { message = "User's email is null." });

            UserModel? currentUser = null;

            try
            {
                currentUser = await _users.GetByEmail(email);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Error", new { message = ex.Message, stackTrace = ex.StackTrace });
            }

            if (currentUser == null)
                RedirectToAction("Index", "Error", new { message = $"User with email \"{email}\" is not found." });

            return View(currentUser);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Index(UserModel editedUser)
        {
            if (ModelState.IsValid == false)
                return ViewValidationError("Index");

            try
            {
                await _users.UpdateUser(editedUser);
                return LocalRedirect("/Home/Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Error", new { message = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        private ViewResult ViewValidationError(string viewName)
        {
            string errorMessages = _validationMessageMaker.GetValidationErrorMessage(ModelState);
            return ViewWarning(viewName, errorMessages);
        }

        private ViewResult ViewWarning(string viewName, string message, string? logMessage = null)
        {
            if (logMessage != null)
                _logger.LogWarning(logMessage);

            ViewBag.ErrorMessage = message;
            return View(viewName);
        }
    }
}
