using Microsoft.AspNetCore.Mvc;
using ToDoList.Logger;
using ToDoList.ViewModels;

namespace ToDoList.Controllers
{
    public class ErrorController : Controller
    {
        private readonly AppLogger _logger;

        public ErrorController(AppLogger logger)
        {
            _logger = logger;
        }

        public IActionResult Index(string message, string stackTrace)
        {
            var viewModel = new ErrorViewModel
            {
                Message = message,
                StackTrace = stackTrace
            };
            _logger.LogError($"Message: {message}\nStackTrace: {stackTrace}");
            return View(viewModel);
        }
    }
}
