using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    public class HomeController : Controller
    {
        private List<TaskModel> _tasks => MockTaskRepository.Tasks;

        public IActionResult Index()
        {
            return View(_tasks);
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
        public IActionResult CreateTask(TaskModel task)
        {
            task.Id = (_tasks.Count + 1).ToString();
            _tasks.Add(task);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateTask(TaskModel task)
        {
            var oldTask = _tasks.Find(t => t.Id == task.Id);
            oldTask.Lable = task.Lable;
            oldTask.Status = task.Status;
            oldTask.ExpiresDate = task.ExpiresDate;
            oldTask.Description = task.Description;
            oldTask.Priority = task.Priority;
            return RedirectToAction("Index");
        }

        public IActionResult DeleteTask(TaskModel task)
        {
            var taskIndex = _tasks.FindIndex(t => t.Id == task.Id);
            _tasks.RemoveAt(taskIndex);
            return RedirectToAction("Index");
        }
    }
}
