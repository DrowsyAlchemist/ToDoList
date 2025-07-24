using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoList.DataBase;
using ToDoList.Logger;
using ToDoList.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ToDoList.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserRepository _users;
        private readonly AppLogger _logger;

        public LoginController(UserRepository userRepository, AppLogger logger, ApplicationDbContext db)
        {
            if (db.Users.Any() == false)
                InitDataBase(db);

            _users = userRepository;
            _logger = logger;
        }

        public IActionResult Login()
        {
            if (HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated)
                return LocalRedirect("/Home/Index");

            return View();
        }

        public ViewResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginData loginData)
        {
            if (ModelState.IsValid == false)
                return ViewValidationError("Login");

            var userInDb = await _users.GetByEmail(loginData.Email);

            if (userInDb == null)
                return ViewWarning("Login", $"User with email \"{loginData.Email}\" has not found.");

            if (userInDb.LoginData.Password.Equals(loginData.Password) == false)
                return ViewWarning("Login", "Incorrect password", $"{loginData.Email} could not logged in. Password is incorrect.");

            return await LogUserIn(loginData, userInDb.Role);
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(UserModel userModel)
        {
            if (ModelState.IsValid == false)
                return ViewValidationError("SignUp");

            LoginData loginData = userModel.LoginData;
            var userInDb = await _users.GetByEmail(loginData.Email);

            if (userInDb != null)
                return ViewWarning("SignUp", $"{loginData.Email} could not signed up. User already exists.");

            userModel.Role = Role.User;
            await _users.AddUser(userModel);
            return await LogUserIn(loginData, Role.User);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return LocalRedirect("/Login/Login");
        }

        public ViewResult DenyAccess()
        {
            var user = HttpContext.User.Identity;
            var path = HttpContext.Request.Path;
            _logger.LogWarning($"Access denied.\nPath: {path}\nUser: {user?.Name}\nAdmin: {HttpContext.User.IsInRole(Role.Admin)}");
            return View();
        }

        private ViewResult ViewValidationError(string viewName)
        {
            string errorMessages = "";

            foreach (var item in ModelState)
                if (item.Value.ValidationState == ModelValidationState.Invalid)
                    foreach (var error in item.Value.Errors)
                        errorMessages = $"{errorMessages}{error.ErrorMessage}\n";

            return ViewWarning(viewName, errorMessages);
        }

        private ViewResult ViewWarning(string viewName, string message, string? logMessage = null)
        {
            if (logMessage != null)
                _logger.LogWarning(logMessage);

            ViewBag.ErrorMessage = message;
            return View(viewName);
        }

        private async Task<IActionResult> LogUserIn(LoginData loginData, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, loginData.Email) ,
                new Claim(ClaimTypes.Role, role),
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));
            _logger.LogInfo($"{loginData.Email} has successfully logged in.");
            return LocalRedirect("/Home/Index");
        }

        private void InitDataBase(ApplicationDbContext db)
        {
            LoginData user1LoginData = new LoginData
            {
                Email = "michael@gmail.com",
                Password = "1234"
            };

            LoginData user2LoginData = new LoginData
            {
                Email = "bob@gmail.com",
                Password = "12345"
            };

            UserModel user1 = new UserModel
            {
                Name = "Michael",
                Role = Role.Admin,
                LoginData = user1LoginData
            };
            UserModel user2 = new UserModel
            {
                Name = "Bob",
                Role = Role.User,
                LoginData = user2LoginData
            };

            TaskModel task1 = new TaskModel
            {
                Lable = "Учить англиский",
                Status = TasksStatus.Active,
                Priority = Priority.Low,
                ExpiresDate = DateTime.Now + TimeSpan.FromDays(1),
                User = user1
            };
            TaskModel task2 = new TaskModel
            {
                Lable = "Учить японский",
                Status = TasksStatus.Pending,
                Priority = Priority.Medium,
                ExpiresDate = DateTime.Now + TimeSpan.FromDays(2),
                User = user1
            };
            TaskModel task3 = new TaskModel
            {
                Lable = "Программировать",
                Status = TasksStatus.Done,
                Priority = Priority.High,
                ExpiresDate = DateTime.Now + TimeSpan.FromHours(5),
                User = user1
            };
            TaskModel task4 = new TaskModel
            {
                Lable = "Сделать зарядку",
                Status = TasksStatus.Done,
                Priority = Priority.Medium,
                ExpiresDate = DateTime.Now + TimeSpan.FromHours(1),
                User = user1
            };
            TaskModel task5 = new TaskModel
            {
                Lable = "Помыть посуду",
                Status = TasksStatus.Pending,
                Priority = Priority.Low,
                ExpiresDate = DateTime.Now + TimeSpan.FromHours(7),
                User = user1
            };
            TaskModel task6 = new TaskModel
            {
                Lable = "Приготовить ужин",
                Status = TasksStatus.Cancelled,
                Priority = Priority.Medium,
                ExpiresDate = DateTime.Now + TimeSpan.FromSeconds(15),
                User = user1
            };
            TaskModel task7 = new TaskModel
            {
                Lable = "Погулять с собакой",
                Status = TasksStatus.Pending,
                Priority = Priority.High,
                ExpiresDate = DateTime.Now + TimeSpan.FromMinutes(35),
                User = user1
            };

            db.Users.AddRange(user1, user2);
            db.Tasks.AddRange(task1, task2, task3, task4, task5, task6, task7);
            db.SaveChanges();
        }
    }
}
