using ToDoList.Models;

namespace ToDoList.ViewModels
{
    public class AdminModel
    {
        public List<UserModel> Users { get; private set; }
        public List<TaskModel> Tasks { get; private set; }

        public AdminModel(List<UserModel> users, List<TaskModel> tasks)
        {
            Users = users;
            Tasks = tasks;
        }
    }
}
