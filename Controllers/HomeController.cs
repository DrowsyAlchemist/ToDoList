using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    public class HomeController : Controller
    {
        private readonly List<TaskModel> _tasks =
          [
              new TaskModel(){Id="1", Lable="Англ", Status=Status.Active, ExpiresDate=DateTime.Now, Description="Meow"},
            new TaskModel(){Id="2", Lable="Японский", Status=Status.Pending, ExpiresDate=DateTime.Now, Description="Meow"},
            new TaskModel(){Id="3", Lable="Прогать", Status=Status.Done, ExpiresDate=DateTime.Now, Description="Meow"}
          ];

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
                oldTask = task;
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
