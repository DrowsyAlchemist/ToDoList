using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoList.DataBase;
using ToDoList.Logger;
using ToDoList.Models;
using Microsoft.AspNetCore.Authentication;

namespace ToDoList.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserRepository _users;
        private readonly AppLogger _logger;

        public LoginController(UserRepository userRepository, AppLogger logger)
        {
            _users = userRepository;
            _logger = logger;
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginData loginData)
        {
            if (loginData == null)
            {
                _logger.LogWarning("loginData is null.");
                return View();
            }
            if (string.IsNullOrEmpty(loginData.Email))
            {
                _logger.LogWarning("Email is null or empty.");
                return View();
            }
            if (string.IsNullOrEmpty(loginData.Password))
            {
                _logger.LogWarning("Password is null or empty.");
                return View();
            }
            var userInDb = await _users.GetByEmail(loginData.Email);

            if (userInDb == null)
            {
                _logger.LogWarning($"{loginData.Email} could not logged in. User has not found.");
                //return LocalRedirect("/Login/Login");
                return Unauthorized();
            }
            if (userInDb.LoginData.Password.Equals(loginData.Password) == false)
            {
                _logger.LogWarning($"{loginData.Email} could not logged in. Password is incorrect.");
                return LocalRedirect("/Login/Login");
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, loginData.Email) ,
                new Claim(ClaimTypes.Role, userInDb.Role),
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));
            _logger.LogInfo($"{loginData.Email} has successfully logged in.");
            return LocalRedirect("/Home/Index");
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(UserModel userModel)
        {
            string form = "";
            foreach (var item in Request.Form)
            {
                form += $"{item.Key} - {item.Value}\n";
            }
            _logger.LogWarning(form);
            var loginData = userModel.LoginData;

            if (loginData == null)
            {
                _logger.LogWarning("loginData is null.");
                return View();
            }
            if (string.IsNullOrEmpty(loginData.Email))
            {
                _logger.LogWarning("Email is null or empty.");
                return View();
            }
            if (string.IsNullOrEmpty(loginData.Password))
            {
                _logger.LogWarning("Password is null or empty.");
                return View();
            }
            var userInDb = await _users.GetByEmail(loginData.Email);

            if (userInDb != null)
            {
                _logger.LogWarning($"{loginData.Email} could not signed up. User already exists.");
                return View();
            }

            userModel.Role = Role.User;
            await _users.AddUser(userModel);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, loginData.Email) ,
                new Claim(ClaimTypes.Role, Role.User),
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));
            _logger.LogInfo($"{loginData.Email} has successfully logged in.");
            return LocalRedirect("/Home/Index");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return LocalRedirect("/Login/Login");
        }

        public IActionResult DenyAccess()
        {
            var user = HttpContext.User.Identity;
            var path = HttpContext.Request.Path;
            _logger.LogWarning($"Access denied.\nPath: {path}\nUser: {user?.Name}\nAdmin: {HttpContext.User.IsInRole(Role.Admin)}");
            return View();
        }
    }
}
