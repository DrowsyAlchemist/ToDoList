using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoList.DataBase;
using ToDoList.Logger;
using ToDoList.Models;
using ToDoList.Services;
using ToDoList.ViewModels;

namespace ToDoList.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserRepository _users;
        private readonly AppLogger _logger;
        private static int _pageSize = 10;

        public HomeController(UserRepository usersRepository, AppLogger appLogger)
        {
            _users = usersRepository;
            _logger = appLogger;
        }

        [Authorize]
        public async Task<IActionResult> Index(TasksOrganizationInfo organizationInfo, UserViewModel userViewModel)
        {
            foreach (var v in Request.Query)
                _logger.LogInfo(v.Key + " - " + v.Value);

            UserModel currentUser = await GetCurrentUser();
            UserModel userToView;

            if (string.IsNullOrEmpty(userViewModel.UserId) == false)
                userToView = await _users.GetById(userViewModel.UserId);
            else
                userToView = currentUser;

            IEnumerable<TaskModel>? userTasks = userToView.Tasks;

            userViewModel.IsAdminMode = currentUser.Role == Role.Admin;
            userViewModel.IsUserTasksOwner = currentUser.Id.Equals(userToView.Id);

            if (userViewModel.IsUserTasksOwner == false)
            {
                if (userViewModel.IsAdminMode == false)
                    throw new InvalidOperationException();

                userViewModel.UserId = userToView.Id;
                userViewModel.Name = userToView.Name;
                userViewModel.Email = userToView.LoginData.Email;
            }
            var indexViewModel = TasksOrganizer.Organize(userTasks, organizationInfo, userViewModel);
            return View("Index", indexViewModel);
        }

        public async Task<IActionResult> ResetFilter(TasksOrganizationInfo organizationInfo, UserViewModel userViewModel)
        {
            organizationInfo.FilterViewModel = null;
            organizationInfo.PageViewModel = new PageViewModel { ItemsPerPage = organizationInfo.PageViewModel.ItemsPerPage };
            userViewModel = new UserViewModel { UserId = userViewModel.UserId };
            return await Index(organizationInfo, userViewModel);
        }

        [Authorize]
        public async Task<IActionResult> SetPageSize(TasksOrganizationInfo organizationInfo, int pageSize, UserViewModel userViewModel)
        {
            foreach (var v in Request.Query)
                _logger.LogInfo(v.Key + " - " + v.Value);

            if (pageSize > 0)
                organizationInfo.PageViewModel = new PageViewModel { ItemsPerPage = pageSize };

            return await Index(organizationInfo, userViewModel);
        }

        [Authorize]
        public IActionResult EditTask(TaskModel task)
        {
            ValidateTask(() => (task == null || string.IsNullOrEmpty(task.Id)));
            return View(task);
        }

        [Authorize]
        public IActionResult CreateTask()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateTask(TaskModel task)
        {
            ValidateTask(() => (task == null));
            var user = await GetCurrentUser();
            await _users.AddTask(task, user);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateTask(TaskModel task)
        {
            ValidateTask(() => (task == null || string.IsNullOrEmpty(task.Id)));
            await _users.UpdateTask(task);
            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> DeleteTask(TaskModel task)
        {
            ValidateTask(() => (task == null || string.IsNullOrEmpty(task.Id)));
            await _users.DeleteTask(task);
            return RedirectToAction("Index");
        }

        //private IActionResult OrganizeTasks(
        //    IEnumerable<TaskModel>? userTasks,
        //    FilterViewModel? filterViewModel = null,
        //    SortState sortState = SortState.DueDateAsc,
        //    int pageNumber = 1,
        //    bool isAdmin = false,
        //    bool canEditTasks = false)
        //{
        //    if (filterViewModel == null)
        //        filterViewModel = new FilterViewModel();

        //    var sortViewModel = new SortViewModel(sortState);
        //    var pageViewModel = new PageViewModel(userTasks.Count(), pageNumber, _pageSize);

        //    if (userTasks.Any())
        //    {
        //        userTasks = FilterTasks(userTasks, filterViewModel);
        //        userTasks = SortTasks(userTasks, sortViewModel);

        //        pageViewModel = new PageViewModel(userTasks.Count(), pageNumber, _pageSize);
        //        userTasks = Paginate(userTasks, pageViewModel);
        //    }
        //    var indexViewModel = new IndexViewModel(userTasks, pageViewModel, filterViewModel, sortViewModel, isAdmin, canEditTasks);
        //    return View(indexViewModel);
        //}

        //private IEnumerable<TaskModel> FilterTasks(IEnumerable<TaskModel> tasks, FilterViewModel filterViewModel)
        //{
        //    if (string.IsNullOrEmpty(filterViewModel.LablePart) == false)
        //        tasks = tasks.Where(t => t.Lable.Contains(filterViewModel.LablePart));

        //    if (filterViewModel.SelectedStatus != null)
        //        tasks = tasks.Where(t => t.Status == filterViewModel.SelectedStatus);

        //    if (filterViewModel.SelectedPriority != null)
        //        tasks = tasks.Where(t => t.Priority == filterViewModel.SelectedPriority);

        //    if (filterViewModel.SelectedDateScope != ViewDateScope.All)
        //        tasks = FilterByDate(tasks, filterViewModel.SelectedDateScope);

        //    return tasks;
        //}

        //private IEnumerable<TaskModel> SortTasks(IEnumerable<TaskModel> tasks, SortViewModel sortViewModel)
        //{
        //    tasks = sortViewModel.Current switch
        //    {
        //        SortState.LableAsc => tasks.OrderBy(t => t.Lable),
        //        SortState.LableDesc => tasks.OrderByDescending(t => t.Lable),
        //        SortState.DueDateAsc => tasks.OrderBy(t => t.ExpiresDate),
        //        SortState.DueDateDesc => tasks.OrderByDescending(t => t.ExpiresDate),
        //        SortState.PriorityAsc => tasks.OrderBy(t => t.Priority),
        //        SortState.PriorityDesc => tasks.OrderByDescending(t => t.Priority),
        //        _ => throw new NotImplementedException(),
        //    };
        //    return tasks;
        //}

        //private IEnumerable<TaskModel> FilterByDate(IEnumerable<TaskModel> tasks, ViewDateScope dateScope)
        //{
        //    var now = DateTime.Now;

        //    tasks = dateScope switch
        //    {
        //        ViewDateScope.Today =>
        //           tasks = tasks.Where(t => t.ExpiresDate.Date == now.Date),
        //        ViewDateScope.Tomorrow =>
        //            tasks = tasks.Where(t => t.ExpiresDate.Date == now.AddDays(1).Date),
        //        ViewDateScope.ThisMonth =>
        //            tasks.Where(t => (t.ExpiresDate.Year == now.Year) && (t.ExpiresDate.Month == now.Month)),
        //        ViewDateScope.All => tasks,
        //        _ => throw new NotImplementedException(),
        //    };
        //    return tasks;
        //}

        //private IEnumerable<TaskModel> Paginate(IEnumerable<TaskModel> tasks, PageViewModel pageViewModel)
        //{
        //    int skipCount = (pageViewModel.PageNumber - 1) * pageViewModel.ItemsPerPage;
        //    tasks = tasks.Skip(skipCount).Take(pageViewModel.ItemsPerPage);
        //    return tasks;
        //}

        private async Task<UserModel> GetCurrentUser()
        {
            var userInApp = HttpContext.User.Identity;
            var currentUser = await _users.GetByEmail(userInApp.Name);
            return currentUser;
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
