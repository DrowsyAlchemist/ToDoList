using Microsoft.EntityFrameworkCore;
using ToDoList.Models;

namespace ToDoList.DataBase
{
    public class UserRepository
    {
        private readonly ApplicationDbContext _context;
        private DbSet<UserModel> _users => _context.Users;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserModel> GetFirst() // Temporary
        {
            return await _users.FirstAsync();
        }

        public async Task<UserModel> GetById(string id)
        {
            ArgumentNullException.ThrowIfNull(id);
            var user = await _users.FirstOrDefaultAsync(u => u.Id.Equals(id));

            if (user == null)
                throw new InvalidOperationException("Пользователь не найден.");

            return user;
        }

        public async Task<List<UserModel>> GetAll()
        {
            return await _users.ToListAsync();
        }

        public async Task<UserModel> AddUser(UserModel user)
        {
            user.Id = Guid.NewGuid().ToString();
            await _users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<UserModel> UpdateUser(UserModel user)
        {
            var oldUser = await _users.FirstOrDefaultAsync(u => u.Id.Equals(user.Id));
            oldUser.Name = user.Name;
            oldUser.Avatar = user.Avatar;
            oldUser.LoginData.Email = user.LoginData.Email;
            oldUser.LoginData.Password = user.LoginData.Password;
            await _context.SaveChangesAsync();
            return oldUser;
        }

        public async Task<UserModel> DeleteUser(UserModel user)
        {
            var userToRemove = await _users.FirstOrDefaultAsync();

            _users.Remove(userToRemove);
            await _context.SaveChangesAsync();
            return userToRemove;
        }
    }
}
