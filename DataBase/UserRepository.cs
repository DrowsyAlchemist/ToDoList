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

        public async Task<UserModel?> GetById(string id)
        {
            ArgumentNullException.ThrowIfNull(id);
            var user = await _users.FirstOrDefaultAsync(u => u.Id.Equals(id));
            return user;
        }

        public async Task<UserModel?> GetByEmail(string email)
        {
            ArgumentNullException.ThrowIfNull(email);
            var user = await _users.FirstOrDefaultAsync(u => u.LoginData.Email.Equals(email));
            return user;
        }

        public async Task<List<UserModel>> GetAll()
        {
            return await _users.ToListAsync();
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

            if(oldUser==null)
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
    }
}
