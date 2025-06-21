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

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<UserModel>(b =>
        //    {
        //        string id = Guid.NewGuid().ToString();
        //        b.HasData(new
        //        {
        //            Id = id,
        //            Name = "Michael",
        //            Role = Role.Admin,
        //        });
        //        b.OwnsOne(u => u.LoginData).HasData(new
        //        {
        //            Email = "michael@gmail.com",
        //            Password = "1234",
        //            UserModelId = id,
        //        });
        //        b.OwnsMany(u => u.Tasks);
        //    });

        //}
    }
}
