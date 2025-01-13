using Microsoft.AspNetCore.Mvc;
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

        [Route("Home/EditTask")]
        [Route("Home/CreateTask")]
        public IActionResult EditTask(TaskModel? task)
        {
            return View(task);
        }

        public IActionResult SaveTask(TaskModel task)
        {
            if (task.Id == null)
            {
                task.Id = "new";
                _tasks.Add(task);
            }
            else
            {
                var oldTask = _tasks.Find(t => t.Id == task.Id);
                oldTask.Lable=task.Lable;
                oldTask.Status = task.Status;
                oldTask.ExpiresDate = task.ExpiresDate;
                oldTask.Description = task.Description;
                oldTask.Priority = task.Priority;
            }
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
