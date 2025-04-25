using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ToDoList.DataBase;
using ToDoList.Infrastructure;
using ToDoList.Logger;
using ToDoList.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddControllersWithViews(opts =>
{
    opts.ModelBinderProviders.Insert(0, new CustomDateTimeModelBinderProvider());
});

string? connection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connection));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) // Аутентификация с помощью Cookies
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/login/denyAccess";
    });
builder.Services.AddAuthorization(); 


builder.Services.AddTransient<UserRepository>();
builder.Services.AddTransient<TaskRepository>();
builder.Services.AddSingleton<AppLogger>(); // Логгирование


var app = builder.Build();

app.UseAuthentication();   // добавление middleware аутентификации 
app.UseAuthorization();   // добавление middleware авторизации 


if (app.Environment.IsDevelopment()) // Обработка ошибок
    app.UseDeveloperExceptionPage();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}");


//initDataBase();


app.Run();


//static void initDataBase(ApplicationDbContext db)
//{
//    UserModel user1 = new UserModel
//    {
//        Id = Guid.NewGuid().ToString(),
//        Name = "Michael",
//        Role = Role.Admin,
//        LoginData = new LoginData { Id = "1", Email = "michael@gmail.com", Password = "1234" }
//    };

//    UserModel user2 = new UserModel
//    {
//        Id = Guid.NewGuid().ToString(),
//        Name = "Bob",
//        Role = Role.Admin,
//        LoginData = new LoginData { Id = "2", Email = "bob@gmail.com", Password = "12345" }
//    };

//    TaskModel task1 = new TaskModel
//    {
//        Id = Guid.NewGuid().ToString(),
//        Lable = "Англ",
//        Status = Status.Active,
//        Priority = Priority.Low,
//        ExpiresDate = DateTime.Now,
//        Description = "Meow1",
//        User = user1,
//    };

//    TaskModel task2 = new TaskModel
//    {
//        Id = Guid.NewGuid().ToString(),
//        Lable = "Японский",
//        Status = Status.Pending,
//        Priority = Priority.Medium,
//        ExpiresDate = DateTime.Now,
//        Description = "Meow2",
//        User = user2,
//    };

//    TaskModel task3 = new TaskModel
//    {
//        Id = Guid.NewGuid().ToString(),
//        Lable = "Прогать",
//        Status = Status.Done,
//        Priority = Priority.High,
//        ExpiresDate = DateTime.Now,
//        Description = "Meow3",
//        User = user1
//    };

//    db.Users.AddRange(user1, user2);
//    db.Tasks.AddRange(task1, task2, task3);
//    db.SaveChanges();

//    db.Users.First().Tasks.Add(new TaskModel {Id = "4", Lable = "Test" });
//    db.SaveChanges();

//    foreach (var task in db.Tasks)
//    {
//        Console.WriteLine($"{task.Lable} - {task.User.Name}\n");
//    }

//    foreach (var user in db.Users)
//    {
//        Console.WriteLine($"{user.Name}:\n");
//        foreach (var task in user.Tasks)
//            Console.WriteLine($"{task.Lable}\n");
//    }

