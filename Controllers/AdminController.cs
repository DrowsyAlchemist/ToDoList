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
        private readonly AppLogger _logger;

        public AdminController(UserRepository users, TaskRepository taskRepository, AppLogger appLogger)
        {
            _users = users;
            _logger = appLogger;
        }

        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> Index()
        {
            List<UserModel> users = await _users.GetAll();
            return View(new AdminModel(users));
        }

        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            await _users.DeleteUser(userId);
            return RedirectToAction("Index");
        }
    }
}
