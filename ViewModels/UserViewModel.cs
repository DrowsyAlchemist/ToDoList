namespace ToDoList.ViewModels
{
    public class UserViewModel
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsAdminMode { get; set; }
        public bool IsUserTasksOwner { get; set; }
    }
}
