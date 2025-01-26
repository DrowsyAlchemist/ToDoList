using Microsoft.EntityFrameworkCore;
using ToDoList.Models;

namespace ToDoList.DataBase
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; } = null!;
        public DbSet<TaskModel> Tasks { get; set; } = null!;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>(b =>
            {
                string id = Guid.NewGuid().ToString();
                b.HasData(new
                {
                    Id = id,
                    Name = "Michael",
                });
                b.OwnsOne(u => u.LoginData).HasData(new
                {
                    Email = "michael@gmail.com",
                    Password = "1234",
                    UserModelId = id,
                });
            });

            modelBuilder.Entity<UserModel>(b =>
            {
                string id = Guid.NewGuid().ToString();
                b.HasData(new
                {
                    Id = id,
                    Name = "Bob",
                });
                b.OwnsOne(u => u.LoginData).HasData(new
                {
                    Email = "bob@gmail.com",
                    Password = "12345",
                    UserModelId = id,
                });
            });


            //modelBuilder.Entity<UserModel>().HasData(
            //    new UserModel()
            //    {
            //        Id = Guid.NewGuid().ToString(),
            //        Name = "Michael",
            //        LoginData = new LoginData() { Email = "michael@gmail.com", Password = "1234" }
            //    },
            //    new UserModel()
            //    {
            //        Id = Guid.NewGuid().ToString(),
            //        Name = "Bob",
            //        LoginData = new LoginData() { Email = "bob@gmail.com", Password = "12345" }
            //    }
            //);

            modelBuilder.Entity<TaskModel>().HasData(
                new TaskModel()
                {
                    Id = Guid.NewGuid().ToString(),
                    Lable = "Англ",
                    Status = Status.Active,
                    Priority = Priority.Low,
                    ExpiresDate = DateTime.Now,
                    Description = "Meow"
                },
                new TaskModel()
                {
                    Id = Guid.NewGuid().ToString(),
                    Lable = "Японский",
                    Status = Status.Pending,
                    Priority = Priority.Medium,
                    ExpiresDate = DateTime.Now,
                    Description = "Meow"
                },
                new TaskModel()
                {
                    Id = Guid.NewGuid().ToString(),
                    Lable = "Прогать",
                    Status = Status.Done,
                    Priority = Priority.High,
                    ExpiresDate = DateTime.Now,
                    Description = "Meow"
                }
            );
        }
    }
}
