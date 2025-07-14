using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoList.DataBase;
using ToDoList.Logger;
using ToDoList.Models;
using ToDoList.ViewModels;

namespace ToDoList.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserRepository _users;

        public AdminController(UserRepository users, TaskRepository taskRepository, AppLogger appLogger)
        {
            _users = users;
        }

        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> Index()
        {
            List<UserModel> users = null!;

            try
            {
                users = await _users.GetAll();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Error", new { message = ex.Message, stackTrace = ex.StackTrace });
            }
            if (users == null || users.Any() == false)
                return RedirectToAction("Index", "Error", new { message = "Users is null or empty. (AdminController.Index)" });

            return View(new AdminModel(users));
        }

        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            try
            {
                await _users.DeleteUser(userId);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Error", new { message = ex.Message, stackTrace = ex.StackTrace });
            }
            return RedirectToAction("Index");
        }
    }
}
