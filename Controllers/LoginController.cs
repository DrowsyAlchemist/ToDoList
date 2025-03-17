using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System;
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

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return LocalRedirect("/Login/Login");
        }
    }
}
