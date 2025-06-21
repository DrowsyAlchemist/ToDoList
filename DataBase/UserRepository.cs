using Microsoft.EntityFrameworkCore;
using ToDoList.Models;

namespace ToDoList.DataBase
{
    public class UserRepository
    {
        private readonly ApplicationDbContext _context;

        private DbSet<UserModel> _users => _context.Users;
        private DbSet<TaskModel> _tasks => _context.Tasks;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserModel> GetFirst() // Temporary
        {
            return await _users.FirstAsync();
        }

        public async Task<UserModel?> GetById(string id)
        {
            ArgumentNullException.ThrowIfNull(id);
            var user = await _users
                .Include(u => u.LoginData)
                .Include(u => u.Tasks)
                .FirstOrDefaultAsync(u => u.Id.Equals(id));
            return user;
        }

        public async Task<UserModel?> GetByEmail(string email)
        {
            ArgumentNullException.ThrowIfNull(email);

            var user = await _users
                .Include(u => u.LoginData)
                .Include(u => u.Tasks)
                .FirstOrDefaultAsync(u => u.LoginData.Email.Equals(email));
            return user;
        }

        public async Task<List<UserModel>> GetAll()
        {
            return await _users
                .Include(u => u.LoginData)
                .Include(u => u.Tasks)
                .ToListAsync();
        }

        public async Task<UserModel> AddUser(UserModel user)
        {
            ArgumentNullException.ThrowIfNull(user);
            user.Id = Guid.NewGuid().ToString();
            await _users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<UserModel> UpdateUser(UserModel user)
        {
            ArgumentNullException.ThrowIfNull(user);
            var oldUser = await GetById(user.Id);

            if (oldUser == null)
                throw new InvalidOperationException("Пользователь не найден.");

            oldUser.Name = user.Name;
            oldUser.Avatar = user.Avatar;
            oldUser.LoginData.Email = user.LoginData.Email;
            oldUser.LoginData.Password = user.LoginData.Password;
            await _context.SaveChangesAsync();
            return oldUser;
        }

        public async Task<UserModel> DeleteUser(UserModel user)
        {
            ArgumentNullException.ThrowIfNull(user);
            var userToRemove = await GetById(user.Id);

            if (userToRemove == null)
                throw new InvalidOperationException("Пользователь не найден.");

            _users.Remove(userToRemove);
            await _context.SaveChangesAsync();
            return userToRemove;
        }

        public async Task<TaskModel> AddTask(TaskModel task, UserModel user)
        {
            ArgumentNullException.ThrowIfNull(task);
            ArgumentNullException.ThrowIfNull(user);
            task.Id = Guid.NewGuid().ToString();
            user.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<TaskModel> UpdateTask(TaskModel task)
        {
            ArgumentNullException.ThrowIfNull(task);
            var user = await _users
                .Include(u => u.Tasks)
                .FirstOrDefaultAsync(u => u.Id.Equals(task.UserId));
            ArgumentNullException.ThrowIfNull(user);
            var taskInDb = user.Tasks.FirstOrDefault(t => t.Id == task.Id);
            ArgumentNullException.ThrowIfNull(taskInDb);
            taskInDb.Lable = task.Lable;
            taskInDb.Status = task.Status;
            taskInDb.ExpiresDate = task.ExpiresDate;
            taskInDb.Description = task.Description;
            taskInDb.Priority = task.Priority;
            await _context.SaveChangesAsync();
            return taskInDb;
        }

        public async Task<TaskModel> DeleteTask(TaskModel task)
        {
            ArgumentNullException.ThrowIfNull(task);
            var user = await _users.FirstOrDefaultAsync(u => u.Id.Equals(task.UserId));
            ArgumentNullException.ThrowIfNull(user);
            var taskToRemove = user.Tasks.FirstOrDefault(t => t.Id == task.Id);
            ArgumentNullException.ThrowIfNull(taskToRemove);
            user.Tasks.Remove(taskToRemove);
            await _context.SaveChangesAsync();
            return task;
        }
    }
}
