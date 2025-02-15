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

        public async Task<IActionResult> Index(AppLogger logger)
        {
            logger.LogInfo("Meow");
            List<TaskModel> tasks = await _tasks.GetAll(); 
            return View(tasks);
        }

        public IActionResult EditTask(TaskModel? task)
        {
            return View(task);
        }

        public IActionResult CreateTask()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask(TaskModel task)
        {
            await _tasks.AddTask(task);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTask(TaskModel task)
        {
            await _tasks.UpdateTask(task);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteTask(TaskModel task)
        {
            await _tasks.DeleteTask(task);
            return RedirectToAction("Index");
        }
    }
}
