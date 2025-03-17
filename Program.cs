using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ToDoList.DataBase;
using ToDoList.Logger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
//builder.Services.AddControllersWithViews(opts =>
//{
//    opts.ModelBinderProviders.Insert(0, new CustomDateTimeModelBinderProvider());
//});

string? connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connection));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) // Аутентификация с помощью Cookies
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/accessDenied";
    });
builder.Services.AddAuthorization(); 


builder.Services.AddTransient<TaskRepository>();
builder.Services.AddTransient<UserRepository>();
builder.Services.AddSingleton<AppLogger>(); // Логгирование


var app = builder.Build();

app.UseAuthentication();   // добавление middleware аутентификации 
app.UseAuthorization();   // добавление middleware авторизации 


if (app.Environment.IsDevelopment()) // Обработка ошибок
    app.UseDeveloperExceptionPage();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}");

app.Run();
