namespace ToDoList.Models
{
    public class TaskSet
    {
        private readonly List<TaskModel> _tasks = new();

        //public async Task<TaskModel> GetById(string id)
        //{
        //    ArgumentNullException.ThrowIfNull(id);
        //    var task = await _tasks.FirstOrDefaultAsync(t => t.Id.Equals(id));

        //    if (task == null)
        //        throw new InvalidOperationException("Пользователь не найден.");

        //    return task;
        //}

        //public IReadOnlyList<TaskModel> GetAll()
        //{
        //    return _tasks;
        //}

        //public TaskModel AddTask(TaskModel task)
        //{
        //    ArgumentNullException.ThrowIfNull(task);
        //    _tasks.Add(task);
        //    return task;
        //}

        //public async Task<TaskModel> UpdateTask(TaskModel task)
        //{
        //    ArgumentNullException.ThrowIfNull(task);
        //    var oldTask = await _tasks.FirstOrDefaultAsync(t => t.Id.Equals(task.Id));

        //    if (task == null)
        //        throw new InvalidOperationException("Задача не найдена.");

        //    oldTask.Lable = task.Lable;
        //    oldTask.Status = task.Status;
        //    oldTask.ExpiresDate = task.ExpiresDate;
        //    oldTask.Description = task.Description;
        //    oldTask.Priority = task.Priority;
        //    await _context.SaveChangesAsync();
        //    return oldTask;
        //}

        //public async Task<TaskModel> DeleteTask(TaskModel task)
        //{
        //    ArgumentNullException.ThrowIfNull(task);
        //    var taskToRemove = await _tasks.FirstOrDefaultAsync();

        //    if (taskToRemove == null)
        //        throw new InvalidOperationException("Задача не найдена.");

        //    _tasks.Remove(taskToRemove);
        //    await _context.SaveChangesAsync();
        //    return taskToRemove;
        //}
    }
}
