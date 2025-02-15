using Microsoft.AspNetCore.Mvc;
using ToDoList.DataBase;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserRepository _users;

        public ProfileController(UserRepository userRepository)
        {
            _users = userRepository;
        }

        public async Task<IActionResult> Index()
        {
            UserModel userModel = await _users.GetFirst(); // Temporary
            return View(userModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(UserModel editedUser)
        {
            await _users.UpdateUser(editedUser);
            return View(editedUser);
        }
    }
}
