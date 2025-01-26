using Microsoft.EntityFrameworkCore;
using ToDoList.Models;

namespace ToDoList.DataBase
{
    public class TaskRepository
    {
        private readonly ApplicationDbContext _context;
        private DbSet<TaskModel> _tasks => _context.Tasks;

        public TaskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TaskModel> GetById(string id)
        {
            ArgumentNullException.ThrowIfNull(id);
            var task = await _tasks.FirstOrDefaultAsync(t => t.Id.Equals(id));

            if (task == null)
                throw new InvalidOperationException("Пользователь не найден.");

            return task;
        }

        public async Task<List<TaskModel>> GetAll()
        {
            return await _tasks.ToListAsync();
        }

        public async Task<TaskModel> AddTask(TaskModel task)
        {
            task.Id = Guid.NewGuid().ToString();
            await _tasks.AddAsync(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<TaskModel> UpdateTask(TaskModel task)
        {
            var oldTask = await _tasks.FirstOrDefaultAsync(t => t.Id.Equals(task.Id));
            oldTask.Lable = task.Lable;
            oldTask.Status = task.Status;
            oldTask.ExpiresDate = task.ExpiresDate;
            oldTask.Description = task.Description;
            oldTask.Priority = task.Priority;
            await _context.SaveChangesAsync();
            return oldTask;
        }

        public async Task<TaskModel> DeleteTask(TaskModel task)
        {
            var taskToRemove = await _tasks.FirstOrDefaultAsync();

            _tasks.Remove(taskToRemove);
            await _context.SaveChangesAsync();
            return taskToRemove;
        }
    }
}
