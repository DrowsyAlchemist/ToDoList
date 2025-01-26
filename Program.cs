using Microsoft.EntityFrameworkCore;
using ToDoList.DataBase;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
//builder.Services.AddControllersWithViews(opts =>
//{
//    opts.ModelBinderProviders.Insert(0, new CustomDateTimeModelBinderProvider());
//});

string connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connection));
builder.Services.AddTransient<TaskRepository>();
builder.Services.AddTransient<UserRepository>();

var app = builder.Build();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");

app.Run();
