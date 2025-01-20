using Microsoft.AspNetCore.Mvc;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    public class ProfileController : Controller
    {
        private List<UserModel> _users => MockUsersRepository.Users;

        public IActionResult Index()
        {
            return View(_users.First());
        }
    }
}
