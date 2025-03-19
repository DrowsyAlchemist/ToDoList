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
        private readonly TaskRepository _tasks;
        private readonly AppLogger _logger;

        public AdminController(UserRepository users, TaskRepository taskRepository, AppLogger appLogger)
        {
            _users = users;
            _tasks = taskRepository;
            _logger = appLogger;
        }

        [Authorize(Roles = nameof(Role.Admin))]
        public async Task<IActionResult> Index()
        {
            List<UserModel> users = await _users.GetAll();
            List<TaskModel> tasks = await _tasks.GetAll();
            return View(new AdminModel(users, tasks));
        }
    }
}
