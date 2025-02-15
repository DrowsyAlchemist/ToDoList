using Microsoft.AspNetCore.Mvc;
using ToDoList.DataBase;
using ToDoList.Logger;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    public class HomeController : Controller
    {
        private readonly TaskRepository _tasks;

        public HomeController(TaskRepository taskRepository)
        {
            _tasks = taskRepository;
        }

        public async Task<IActionResult> Index()
        {
            List<TaskModel> tasks = await _tasks.GetAll();
            return View(tasks);
        }

        public IActionResult EditTask(TaskModel task, AppLogger appLogger)
        {
            ValidateTask(() => (task == null || string.IsNullOrEmpty(task.Id)), appLogger);
            return View(task);
        }

        public IActionResult CreateTask()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask(TaskModel task, AppLogger appLogger)
        {
            ValidateTask(() => (task == null), appLogger);
            await _tasks.AddTask(task);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTask(TaskModel task, AppLogger appLogger)
        {
            ValidateTask(() => (task == null || string.IsNullOrEmpty(task.Id)), appLogger);
            await _tasks.UpdateTask(task);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteTask(TaskModel task, AppLogger appLogger)
        {
            ValidateTask(() => (task == null || string.IsNullOrEmpty(task.Id)), appLogger);
            await _tasks.DeleteTask(task);
            return RedirectToAction("Index");
        }

        private void ValidateTask(Func<bool> condition, AppLogger appLogger)
        {
            if (condition.Invoke())
            {
                appLogger.LogError("Task is null or incorrect.");
                throw new ArgumentNullException("Task");
            }
        }
    }
}
