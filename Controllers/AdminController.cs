using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;
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

        public async Task<IActionResult> DeleteUser(UserModel user)
        {
            await _users.DeleteUser(user);
            return RedirectToAction("Index");
        }

        //public IActionResult ViewUserTasks(UserModel user)
        //{
        //    _logger.LogWarning("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
        //    _logger.LogWarning(user.Name);
        //    return RedirectToActionPreserveMethod("ViewUserTasks", "Admin", new { user });
        //    //foreach (var v in Request.Query)
        //    //    _logger.LogInfo(v.Key + " - " + v.Value);

        //    //var userInDb = await _users.GetById(user.Id);
        //    //return View(userInDb);
        //}

    }
}
