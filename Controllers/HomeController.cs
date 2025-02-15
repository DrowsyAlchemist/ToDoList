using Microsoft.AspNetCore.Mvc;
using ToDoList.DataBase;
using ToDoList.Logger;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    public class HomeController : Controller
    {
        private readonly TaskRepository _tasks;
        private readonly AppLogger _logger;

        public HomeController(TaskRepository taskRepository, AppLogger appLogger)
        {
            _tasks = taskRepository;
            _logger = appLogger;
        }

        public async Task<IActionResult> Index()
        {
            List<TaskModel> tasks = await _tasks.GetAll();
            return View(tasks);
        }

        public IActionResult EditTask(TaskModel task)
        {
            ValidateTask(() => (task == null || string.IsNullOrEmpty(task.Id)));
            return View(task);
        }

        public IActionResult CreateTask()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask(TaskModel task)
        {
            ValidateTask(() => (task == null));
            await _tasks.AddTask(task);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTask(TaskModel task)
        {
            ValidateTask(() => (task == null || string.IsNullOrEmpty(task.Id)));
            await _tasks.UpdateTask(task);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteTask(TaskModel task)
        {
            ValidateTask(() => (task == null || string.IsNullOrEmpty(task.Id)));
            await _tasks.DeleteTask(task);
            return RedirectToAction("Index");
        }

        private void ValidateTask(Func<bool> condition)
        {
            if (condition.Invoke())
            {
                _logger.LogError("Task is null or incorrect.");
                throw new ArgumentNullException("Task");
            }
        }
    }
}
