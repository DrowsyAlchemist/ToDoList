using ToDoList.Models;

namespace ToDoList.ViewModels
{
    public class AdminModel
    {
        public List<UserModel> Users { get; private set; }

        public AdminModel(List<UserModel> users)
        {
            Users = users;
        }
    }
}
