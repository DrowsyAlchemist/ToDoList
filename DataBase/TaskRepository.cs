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

        public async Task<TaskModel> AddTask(TaskModel task)
        {
            ArgumentNullException.ThrowIfNull(task);
            task.Id = Guid.NewGuid().ToString();
            await _tasks.AddAsync(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<TaskModel> UpdateTask(TaskModel task)
        {
            ArgumentNullException.ThrowIfNull(task);
            var oldTask = await _tasks.FirstOrDefaultAsync(t => t.Id.Equals(task.Id));

            if (task == null)
                throw new InvalidOperationException("Задача не найдена.");

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
            ArgumentNullException.ThrowIfNull(task);
            var taskToRemove = await _tasks.FirstOrDefaultAsync();

            if (taskToRemove == null)
                throw new InvalidOperationException("Задача не найдена.");

            _tasks.Remove(taskToRemove);
            await _context.SaveChangesAsync();
            return taskToRemove;
        }
    }
}
